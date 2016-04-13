using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Xunit.Sdk
{
    /// <summary>
    /// Implementation of <see cref="ITestCommand"/> which executes the
    /// <see cref="TestResultCallbackCommand"/> instances attached to a test method.
    /// </summary>
    public class TestResultCallbackCommand : DelegatingTestCommand
	{
        IMethodInfo testMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultCallbackCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="testMethod">The method.</param>
        public TestResultCallbackCommand(ITestCommand innerCommand, IMethodInfo testMethod)
			: base(innerCommand)
		{
			this.testMethod = testMethod;
		}

		/// <summary>
		/// Executes the test method.
		/// </summary>
		/// <param name="testClass">The instance of the test class</param>
		/// <returns>Returns information about the test run</returns>
		[SuppressMessage("Microsoft.Usage", "CA2219:DoNotRaiseExceptionsInExceptionClauses", Justification = "We do actually verify this. Promise!")]
		public override MethodResult Execute(object testClass)
		{
			bool testExceptionThrown = false;
		    Exception testException = null;
		    MethodResult testResult = null;
            try
			{
			    testResult = InnerCommand.Execute(testClass);
                return testResult;
			}
			catch (Exception e)
			{
				testExceptionThrown = true;
			    testException = e;
                throw;
			}
			finally
			{
			    if (testExceptionThrown)
			    {
			        testResult = new FailedResult(testMethod, testException, InnerCommand.DisplayName);
			    }

                List<Exception> afterExceptions = new List<Exception>();
                foreach (var attr in GetAfterTestCallbackAttributes())
                {
                    try
                    {
                        attr.After(testMethod.MethodInfo, testResult);
                    }
                    catch (Exception ex)
                    {
                        afterExceptions.Add(ex);
                    }
                }

				if (!testExceptionThrown && afterExceptions.Count > 0)
					throw new AfterTestException(afterExceptions);
			}
		}

		private IEnumerable<TestResultCallbackAttribute> GetAfterTestCallbackAttributes()
		{
		    var methodInfo = testMethod.MethodInfo;

			foreach (TestResultCallbackAttribute testResultCallbackAttribute in
                    methodInfo.GetCustomAttributes(typeof(TestResultCallbackAttribute), true))
			{
				yield return testResultCallbackAttribute;
			}
			if (methodInfo.DeclaringType == null)
				yield break;
			foreach (TestResultCallbackAttribute testResultCallbackAttribute in
                    methodInfo.DeclaringType.GetCustomAttributes(typeof(TestResultCallbackAttribute), true))
			{
				yield return testResultCallbackAttribute;
			}
			foreach (TestResultCallbackAttribute testResultCallbackAttribute in
                    methodInfo.DeclaringType.Assembly.GetCustomAttributes(typeof(TestResultCallbackAttribute), true))
			{
				yield return testResultCallbackAttribute;
			}
		}
	}
}