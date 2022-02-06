using System;

namespace CloudSeedApp;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base() { }
}