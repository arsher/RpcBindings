﻿using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling.Delegates;
using Moq;
using System;
using System.Runtime.CompilerServices;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Marshaling;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling.Delegates
{
    public class DelegateContainerBaseTests
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Sun(ICallbackExecutor<object> callbackExecutor)
        {
            new DelegateContainerBase<object>(1, callbackExecutor, ctx => { }, typeof(object), typeof(Action));
        }
        
        [Fact]
        public void CallbackDeleted()
        {
            var callbackExecutor = Mock.Of<ICallbackExecutor<object>>();
            var callbackExecutorMock = Mock.Get(callbackExecutor);
            callbackExecutorMock.SetupGet(_ => _.CanExecute).Returns(true);
            Sun(callbackExecutor);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            callbackExecutorMock.Verify(c => c.DeleteCallback(1));
        }
    }
}
