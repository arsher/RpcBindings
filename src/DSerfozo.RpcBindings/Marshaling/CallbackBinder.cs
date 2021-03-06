﻿using System;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class CallbackBinder<TMarshal>
    {
        private readonly BindingDelegate<TMarshal> next;
        private readonly ICallbackFactory<TMarshal> callbackFactory;

        public CallbackBinder(BindingDelegate<TMarshal> next, ICallbackFactory<TMarshal> callbackFactory)
        {
            this.next = next;
            this.callbackFactory = callbackFactory;
        }

        public void Bind(BindingContext<TMarshal> ctx)
        {
            var isTypedCallback = ctx.TargetType != null && typeof(MulticastDelegate).IsAssignableFrom(ctx.TargetType);
            var isCallback = ctx.TargetType == typeof(ICallback);
            if (isTypedCallback || isCallback)
            {
                var delegateType = ctx.TargetType;
                ctx.TargetType = typeof(CallbackDescriptor);

                next(ctx);

                if ((ctx.ObjectValue as CallbackDescriptor)?.FunctionId > 0)
                {
                    var desc = (CallbackDescriptor)ctx.ObjectValue;
                    try
                    {
                        ctx.ObjectValue = callbackFactory.CreateCallback(desc.FunctionId,
                            isTypedCallback ? delegateType : null, ctx.Binder);
                    }
                    catch (InvalidOperationException)
                    {
                        ctx.ObjectValue = null;
                    }
                }
            }
            else
            {
                next(ctx);
            }
        }
    }
}
