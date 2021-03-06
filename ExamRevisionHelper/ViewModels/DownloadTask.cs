﻿using Prism.Mvvm;

namespace ExamRevisionHelper.ViewModels
{
    public enum DownloadTaskState { Pending, Downloading, Completed, Error }
    public class DownloadTask : BindableBase
    {
        public string FileName { get; set; }

        #region Property: Progress
        private byte _progress;
        public byte Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }
        #endregion

        #region Property: State
        private DownloadTaskState _state;
        public DownloadTaskState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
        #endregion

        public string ResourceUrl { get; set; }

        public string LocalPath { get; set; }
    }
}
