using System.Collections.Generic;
using System.Threading;

namespace DoxygenGenerator
{
    /// <summary>
    ///  This class encapsulates the data output by Doxygen so it can be shared with Unity in a thread share way.	 
    /// </summary>
    public class DoxygenThreadSafeOutput
    {
        private ReaderWriterLockSlim outputLock = new ReaderWriterLockSlim();
        private string CurrentOutput = "";
        private List<string> FullLog = new List<string>();
        private bool Finished = false;
        private bool Started = false;

        public string ReadLine()
        {
            outputLock.EnterReadLock();
            try
            {
                return CurrentOutput;
            }
            finally
            {
                outputLock.ExitReadLock();
            }
        }

        public void SetFinished()
        {
            outputLock.EnterWriteLock();
            try
            {
                Finished = true;
            }
            finally
            {
                outputLock.ExitWriteLock();
            }
        }

        public void SetStarted()
        {
            outputLock.EnterWriteLock();
            try
            {
                Started = true;
            }
            finally
            {
                outputLock.ExitWriteLock();
            }
        }

        public bool IsStarted()
        {
            outputLock.EnterReadLock();
            try
            {
                return Started;
            }
            finally
            {
                outputLock.ExitReadLock();
            }
        }

        public bool IsFinished()
        {
            outputLock.EnterReadLock();
            try
            {
                return Finished;
            }
            finally
            {
                outputLock.ExitReadLock();
            }
        }

        public List<string> ReadFullLog()
        {
            outputLock.EnterReadLock();
            try
            {
                return FullLog;
            }
            finally
            {
                outputLock.ExitReadLock();
            }
        }

        public void WriteFullLog(List<string> newLog)
        {
            outputLock.EnterWriteLock();
            try
            {
                FullLog = newLog;
            }
            finally
            {
                outputLock.ExitWriteLock();
            }
        }

        public void WriteLine(string newOutput)
        {
            outputLock.EnterWriteLock();
            try
            {
                CurrentOutput = newOutput;
            }
            finally
            {
                outputLock.ExitWriteLock();
            }
        }
    }
}
