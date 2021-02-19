﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Core
{
    public enum ErrorType { SubjectListUpdateFailed, SubjectRepoUpdateFailed, SubjectNotSupported, Other }
    public enum NotificationType { Initializing, SubjectListUpdated, SubjectUpdated, Finished }

    public class UpdateServiceNotifiedEventArgs : EventArgs
    {
        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
    }
    public class UpdateServiceErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }

#pragma warning disable CA2237 // Mark ISerializable types with serializable
    public class SubjectUnsupportedException : Exception
    {
        public string[] UnsupportedSubjects { get; set; }

        public SubjectUnsupportedException() { }
        public SubjectUnsupportedException(string msg) : base(msg) { }
    }

#pragma warning restore CA2237 // Mark ISerializable types with serializable

    public static class PastPaperHelperUpdateService
    {
        public static PastPaperHelperCore Instance;

        public delegate void UpdateServiceNotifiedEventHandler(UpdateServiceNotifiedEventArgs args);

        public static event UpdateServiceNotifiedEventHandler UpdateServiceNotifiedEvent;


        public delegate void UpdateServiceErrorEventHandler(UpdateServiceErrorEventArgs args);

        public static event UpdateServiceErrorEventHandler UpdateServiceErrorEvent;


        public delegate void SubjectUnsubscribedEventHandler(Subject subj);

        public static event SubjectUnsubscribedEventHandler SubjectUnsubscribedEvent;


        public delegate void SubjectSubscribedEventHandler(Subject subj);

        public static event SubjectSubscribedEventHandler SubjectSubscribedEvent;


        public delegate void SubjectSubscribeFailedEventHandler(Subject subj);

        public static event SubjectSubscribeFailedEventHandler SubjectSubscribeFailedEvent;


        public static async void UpdateAll(ICollection<string> subscribedSubjects)
        {
            var source = Instance.CurrentSource;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                NotificationType = NotificationType.Initializing,
                Message = $"Loading from {source.DisplayName}..."
            });

            //Download subject list from web server
            try
            {
                await source.UpdateSubjectUrlMapAsync();
                //PastPaperHelperCore.SubjectsLoaded = source.SubjectUrlMap.Keys.ToArray();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to download subject list from {source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {source.DisplayName}.",
                NotificationType = NotificationType.SubjectListUpdated
            });

            //Download papers from web server
            Subject[] subjList = Instance.SubjectsAvailable;
            List<string> failedList = new();
            foreach (var item in subscribedSubjects)
            {
                var found = PastPaperHelperCore.TryFindSubject(item, out _, subjList);
                if (!found) failedList.Add(item);
            }
            if (failedList.Count != 0) UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
            {
                ErrorMessage = $"Syllabus code: {string.Join(',', failedList)} is not supported.",
                ErrorType = ErrorType.SubjectNotSupported,
                Exception = new SubjectUnsupportedException($"Syllabus code: {string.Join(',', failedList)} is not supported.") { UnsupportedSubjects = failedList.ToArray() }
            });

            List<Subject> failed = new();
            foreach (Subject subj in Instance.SubjectsSubscribed)
            {
                try
                {
                    await source.AddOrUpdateSubject(subj);
                }
                catch (Exception e)
                {
                    failed.Add(subj);
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        ErrorMessage = $"Failed to update {subj.Name} from {source.DisplayName}.",
                        ErrorType = ErrorType.SubjectRepoUpdateFailed,
                        Exception = e
                    });
                    continue;
                }

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"{subj.Name} updated from {source.DisplayName}.",
                    NotificationType = NotificationType.SubjectUpdated
                });
            }

            //Update (partially) failed
            if (failed.Count != 0) return;
            //Update successful
            XmlDocument dataDocument = source.SaveDataToXml(Instance.SubscriptionRepo);
            Instance.UserData = dataDocument;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"All subjects updated from {source.DisplayName} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        public static async Task UpdateSubjectList()
        {
            var source = Instance.CurrentSource;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                NotificationType = NotificationType.Initializing,
                Message = $"Loading subject list from {source.DisplayName}..."
            });
            try
            {
                await source.UpdateSubjectUrlMapAsync();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to fetch data from {source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {source.DisplayName} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        public async static Task<bool> SubscribeAsync(Subject subj)
        {
            try
            {
                if (Instance.SubscriptionRepo.ContainsKey(subj)) return false;
                await Instance.CurrentSource?.AddOrUpdateSubject(subj);
            }
            catch (Exception)
            {
                SubjectSubscribeFailedEvent?.Invoke(subj);
                return false;
            }
            SubjectSubscribedEvent?.Invoke(subj);
            return true;
        }

        public static void Unsubscribe(Subject subject)
        {
            Instance.SubscriptionRepo.Remove(subject);
            //TODO: Update XML cache, and make this async
            SubjectUnsubscribedEvent?.Invoke(subject);
        }
    }
}
