﻿using System;
using System.Threading;

namespace Sleipner.Cache.LookupHandlers.Sync
{
    public class RequestWaitHandle<TResult>
    {
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private Exception _thrownException;
        private TResult _result;

        public TResult WaitForResult()
        {
            _resetEvent.WaitOne();
            if (_thrownException != null)
            {
                throw _thrownException;
            }

            return _result;
        }

        public void Release(TResult result)
        {
            _result = result;
            _resetEvent.Set();
        }

        public void ReleaseWithException(Exception exception)
        {
            _thrownException = exception;
            _resetEvent.Set();
        }
    }
}