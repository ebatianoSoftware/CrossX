﻿using System;

namespace CrossX.Framework.IoC
{
    public interface IScopeBuilder
    {
        IScopeBuilder WithType<TType>();
        IScopeBuilder WithType(Type type);
        IScopeBuilder As<TType>();
        IScopeBuilder As(Type type);
        IScopeBuilder AsSingleton();
        IScopeBuilder AsSelf();
        IScopeBuilder WithInstance(object instance);
    }
}
