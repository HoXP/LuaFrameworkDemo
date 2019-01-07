

namespace User.Http.Download
{
    public class DownloadManager
    {
        private static DownloadSingle m_downloadSingle = new DownloadSingle();
        private static DownloadQueue m_downloadqueue = new DownloadQueue();
        private static DownloadMultipleTasks m_downloadMultipleTask = new DownloadMultipleTasks();

        public static bool AddDownloadQueue(DownloadData downloadData)
        {
            if (downloadData == null 
                || string.IsNullOrEmpty(downloadData.savaFilePath)
                || string.IsNullOrEmpty(downloadData.url) 
            ) { return false; }

            if (m_downloadSingle == null || m_downloadqueue == null ||  m_downloadMultipleTask == null)
            {
                return false;
            }
            switch(downloadData.downLoadState)
            {
                case DownLoadState.Single:
                    m_downloadSingle.AddDownloadInfo(downloadData);
                    break;
                case DownLoadState.Queue:
                    m_downloadqueue.AddDownloadInfo(downloadData);
                    break;
                case DownLoadState.MultipleTasks:
                    m_downloadMultipleTask.AddDownloadInfo(downloadData);
                    break;
            }
            return true;
        }

        public static bool StopDownLoadTask(DownloadData downloadData)
        {
            if (m_downloadSingle == null || m_downloadqueue == null || m_downloadMultipleTask == null)
            {
                return false;
            }
            switch (downloadData.downLoadState)
            {
                case DownLoadState.Single:
                    m_downloadSingle.StopDownLoadTask(downloadData);
                    break;
                case DownLoadState.Queue:
                    m_downloadqueue.StopDownLoadTask(downloadData);
                    break;
                case DownLoadState.MultipleTasks:
                    m_downloadMultipleTask.StopDownLoadTask(downloadData);
                    break;
            }
            return true;
        }
        public static void Clear()
        {
            m_downloadSingle.Clear();
            m_downloadqueue.Clear();
            
            m_downloadMultipleTask.Clear();
        }
    }

}