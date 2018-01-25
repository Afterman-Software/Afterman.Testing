namespace Afterman.Testing.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    public sealed class AssertEx
    {
        public static void NoExceptionThrown<T>(Action a) where T : Exception
        {
            try
            {
                a();
            }
            catch (T)
            {
                Assert.Fail($"Expected no {typeof(T).Name} to be thrown");
            }
        }
    }
}
