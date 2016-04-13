using System;
using System.Reflection;
using Xunit.Sdk;

namespace Xunit
{
    /// <summary>
    /// Base attribute which indicates a test method interception (allows code to be run after the test is run).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public abstract class TestResultCallbackAttribute : Attribute
    {

        /// <summary>
        /// This method is called after the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        /// <param name="result">The test result</param>
        public virtual void After(MethodInfo methodUnderTest, MethodResult result) { }

    }
}