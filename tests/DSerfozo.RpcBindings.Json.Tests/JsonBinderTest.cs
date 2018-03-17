﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DSerfozo.RpcBindings.Json.Tests
{
    public class JsonBinderTest
    {
        public class ComplexType
        {
            public string String { get; set; }

            public int Int { get; set; }
        }

        [Fact]
        public void IntConverted()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(Int32),
                Value = JToken.Parse("1") as JToken
            });

            Assert.Equal(1, (int)actual);
        }

        [Fact]
        public void FloatConverted()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(Single),
                Value = JToken.Parse("1.23") as JToken
            });

            Assert.True(Math.Abs(1.23f - (float)actual) < 0.0000001);
        }

        [Fact]
        public void StringConverted()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(string),
                Value = JToken.Parse("\"text\"") as JToken
            });

            Assert.Equal("text", (string)actual);
        }

        [Fact]
        public void ComplexTypeConverted()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(ComplexType),
                Value = JToken.Parse("{\"String\":\"text\", \"Int\": 9}") as JToken
            });

            Assert.Equal("text", ((ComplexType)actual).String);
            Assert.Equal(9, ((ComplexType)actual).Int);
        }

        [Fact]
        public void CallbackConverted()
        {
            var callbackFactory = Mock.Of<ICallbackFactory<JToken>>();
            var callbackFactoryMock = Mock.Get(callbackFactory);
            var binder = new JsonBinder(new JsonSerializer(), callbackFactory);

            callbackFactoryMock.Setup(_ => _.CreateCallback(9, null, binder)).Returns(Mock.Of<ICallback>());

            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(ICallback),
                Value = JToken.Parse("{\"functionId\":9}") as JToken
            });

            Assert.IsAssignableFrom<ICallback>(actual);
        }

        [Fact]
        public void EmptyTargetType()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                Value = JToken.Parse("{\"id\":9, \"sub\":{\"test\":\"sad\"}}") as JToken
            });

            //TODO: this is not a proper assertion and also the code behind this conversion is bad
            Assert.IsAssignableFrom<IDictionary<string, object>>(actual);
        }

        [Fact]
        public void EmptyTargetTypePrimitivePayload()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                Value = JToken.Parse("\"data\"") as JToken
            });

            Assert.Equal("data", (string)actual);
        }

        [Fact]
        public void NullValue()
        {
            var binder = new JsonBinder(new JsonSerializer(), Mock.Of<ICallbackFactory<JToken>>());
            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                
            });

            Assert.Null(actual);
        }

        [Fact]
        public void DelegateTypeGenerated()
        {
            var callbackFactory = Mock.Of<ICallbackFactory<JToken>>();
            var callbackFactoryMock = Mock.Get(callbackFactory);

            var binder = new JsonBinder(new JsonSerializer(), callbackFactory);
            callbackFactoryMock.Setup(_ => _.CreateCallback(9, typeof(Func<string, Task<string>>), binder)).Returns((object)new Func<string, Task<string>>(s => null));

            var actual = binder.BindToNet(new ParameterBinding<JToken>
            {
                TargetType = typeof(Func<string, Task<string>>),
                Value = JToken.Parse("{\"functionId\":9}") as JToken
            });

            Assert.IsAssignableFrom<MulticastDelegate>(actual);
        }
    }
}
