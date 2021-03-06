﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

public class ExceptionAssertsTests
{
    public class DoesNotThrow_Action
    {
        [Fact]
        public void CorrectExceptionType()
        {
            var ex = Assert.Throws<DoesNotThrowException>(
                () => Assert.DoesNotThrow(
                    () => { throw new NotImplementedException("Exception Message"); }));

            Assert.Equal("Assert.DoesNotThrow() Failure", ex.UserMessage);
            Assert.Equal("(No exception)", ex.Expected);
            Assert.Equal("System.NotImplementedException: Exception Message", ex.Actual);
        }

        [Fact]
        public void CodeDoesNotThrow()
        {
            bool methodCalled = false;

            Assert.DoesNotThrow(() => methodCalled = true);

            Assert.True(methodCalled);
        }

        [Fact]
        public void CodeThrows()
        {
            var ex = Record.Exception(
                () => Assert.DoesNotThrow(
                    () => ThrowingMethod()));

            Assert.IsType<DoesNotThrowException>(ex);
            Assert.Contains("NotImplementedException", ex.Message);
        }

        void ThrowingMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class DoesNotThrow_Func
    {
        [Fact]
        public void CodeDoesNotThrow()
        {
            bool methodCalled = false;

            Assert.DoesNotThrow(() => { methodCalled = true; return 0; });

            Assert.True(methodCalled);
        }

        [Fact]
        public void CodeThrows()
        {
            var ex = Record.Exception(
                () => Assert.DoesNotThrow(
                    () => ThrowingMethod()));

            Assert.IsType<DoesNotThrowException>(ex);
            Assert.Contains("NotImplementedException", ex.Message);
        }

        int ThrowingMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class DoesNotThrow_Task
    {
        [Fact]
        public void CodeDoesNotThrow()
        {
            bool methodCalled = false;

            Assert.DoesNotThrow(
                () => Task.Factory.StartNew(
                    () => methodCalled = true));

            Assert.True(methodCalled);
        }

        [Fact]
        public void CodeThrows()
        {
            var ex = Record.Exception(
                () => Assert.DoesNotThrow(
                    () => Task.Factory.StartNew(ThrowingMethod)));

            Assert.IsType<DoesNotThrowException>(ex);
            Assert.Contains("NotImplementedException", ex.Message);
        }

        void ThrowingMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class Throws_Generic_Action
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            try
            {
                Assert.Throws<ArgumentException>(() => { });
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            try
            {
                Assert.Throws<Exception>(() => { throw new InvalidOperationException(); });
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void StackTraceForThrowsIsOriginalThrowNotAssertThrows()
        {
            try
            {
                Assert.Throws<InvalidCastException>(() => ThrowingMethod());
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Contains("Throws_Generic_Action.ThrowingMethod", exception.StackTrace);
                Assert.DoesNotContain("Xunit.Assert", exception.StackTrace);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowingMethod()
        {
            throw new ArgumentException();
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws<ArgumentException>(() => { throw new ArgumentException(); });

            Assert.NotNull(ex);
        }
    }

    public class Throws_Generic_Func
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            var accessor = new StubAccessor();

            try
            {
                Assert.Throws<ArgumentException>(() => accessor.SuccessfulProperty);
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            var accessor = new StubAccessor();

            try
            {
                Assert.Throws<Exception>(() => accessor.FailingProperty);
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void StackTraceForThrowsIsOriginalThrowNotAssertThrows()
        {
            var accessor = new StubAccessor();

            try
            {
                Assert.Throws<InvalidCastException>(() => accessor.FailingProperty);
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Contains("StubAccessor.get_FailingProperty", exception.StackTrace);
                Assert.DoesNotContain("Xunit.Assert", exception.StackTrace);
            }
        }

        [Fact]
        public void GotExpectedException()
        {
            var accessor = new StubAccessor();

            var ex = Assert.Throws<InvalidOperationException>(() => accessor.FailingProperty);

            Assert.NotNull(ex);
        }
    }

    public class Throws_Generic_Task
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            try
            {
                Assert.Throws<ArgumentException>(() => Task.Factory.StartNew(() => { }));
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            try
            {
                Assert.Throws<Exception>(
                    () => Task.Factory.StartNew(
                        () => { throw new InvalidOperationException(); }));
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => Task.Factory.StartNew(
                    () => { throw new ArgumentException(); }));

            Assert.NotNull(ex);
        }
    }

    public class Throws_NonGeneric_Action
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            try
            {
                Assert.Throws(typeof(ArgumentException), () => { });
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            try
            {
                Assert.Throws(typeof(Exception), () => { throw new InvalidOperationException(); });
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => { throw new ArgumentException(); });

            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }
    }

    public class Throws_NonGeneric_Func
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            var accessor = new StubAccessor();

            try
            {
                Assert.Throws(typeof(ArgumentException), () => accessor.SuccessfulProperty);
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            var accessor = new StubAccessor();

            try
            {
                Assert.Throws(typeof(Exception), () => accessor.FailingProperty);
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void GotExpectedException()
        {
            var accessor = new StubAccessor();

            var ex = Assert.Throws(typeof(InvalidOperationException), () => accessor.FailingProperty);

            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
        }
    }

    public class Throws_NonGeneric_Task
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            try
            {
                Assert.Throws(typeof(ArgumentException),
                    () => Task.Factory.StartNew(
                        () => { }));
            }
            catch (AssertActualExpectedException exception)
            {
                Assert.Equal("(No exception was thrown)", exception.Actual);
            }
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            try
            {
                Assert.Throws(typeof(Exception),
                    () => Task.Factory.StartNew(
                        () => { throw new InvalidOperationException(); }));
            }
            catch (XunitException exception)
            {
                Assert.Equal("Assert.Throws() Failure", exception.UserMessage);
            }
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws(typeof(ArgumentException),
                () => Task.Factory.StartNew(
                    () => { throw new ArgumentException(); }));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }
    }

    public class ThrowsArgument_Action
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentException>(
                    "paramName",
                    () => { }));

            var throwsEx = Assert.IsType<ThrowsException>(ex);
            Assert.Equal("(No exception was thrown)", throwsEx.Actual);
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentException>(
                    "paramName",
                    () => { throw new InvalidOperationException(); }));

            Assert.IsType<ThrowsException>(ex);
            Assert.Contains("Assert.Throws() Failure" + Environment.NewLine +
                            "Expected: System.ArgumentException" + Environment.NewLine +
                            "Actual:   System.InvalidOperationException", ex.Message);
        }

        [Fact]
        public void StackTraceForThrowsIsOriginalThrowNotAssertThrows()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentException>(
                    "paramName",
                    () => ThrowingMethod()));

            Assert.Contains("ThrowsArgument_Action.ThrowingMethod", ex.StackTrace);
            Assert.DoesNotContain("Xunit.Assert", ex.StackTrace);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowingMethod()
        {
            throw new InvalidCastException();
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws<ArgumentException>("paramName", () => { throw new ArgumentException("message", "paramName"); });

            Assert.NotNull(ex);
        }

        [Fact]
        public void MismatchedParameterName()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentException>(
                    "paramName",
                    () => { throw new ArgumentException("message", "paramName2"); }));

            var eqEx = Assert.IsType<EqualException>(ex);
            Assert.Equal("paramName", eqEx.Expected);
            Assert.Equal("paramName2", eqEx.Actual);
        }
    }

    public class ThrowsArgumentNull_Action
    {
        [Fact]
        public void ExpectExceptionButCodeDoesNotThrow()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentNullException>(
                    "paramName",
                    () => { }));

            var throwsEx = Assert.IsType<ThrowsException>(ex);
            Assert.Equal("(No exception was thrown)", throwsEx.Actual);
        }

        [Fact]
        public void ExpectExceptionButCodeThrowsDerivedException()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentNullException>(
                    "paramName",
                    () => { throw new InvalidOperationException(); }));

            Assert.IsType<ThrowsException>(ex);
            Assert.Contains("Assert.Throws() Failure" + Environment.NewLine +
                            "Expected: System.ArgumentNullException" + Environment.NewLine +
                            "Actual:   System.InvalidOperationException", ex.Message);
        }

        [Fact]
        public void StackTraceForThrowsIsOriginalThrowNotAssertThrows()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentNullException>(
                    "paramName",
                    () => ThrowingMethod()));

            Assert.Contains("ThrowsArgumentNull_Action.ThrowingMethod", ex.StackTrace);
            Assert.DoesNotContain("Xunit.Assert", ex.StackTrace);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowingMethod()
        {
            throw new InvalidCastException();
        }

        [Fact]
        public void GotExpectedException()
        {
            var ex = Assert.Throws<ArgumentNullException>("paramName", () => { throw new ArgumentNullException("paramName"); });

            Assert.NotNull(ex);
        }

        [Fact]
        public void MismatchedParameterName()
        {
            var ex = Record.Exception(
                () => Assert.Throws<ArgumentNullException>(
                    "paramName",
                    () => { throw new ArgumentNullException("paramName2"); }));

            var eqEx = Assert.IsType<EqualException>(ex);
            Assert.Equal("paramName", eqEx.Expected);
            Assert.Equal("paramName2", eqEx.Actual);
        }
    }

    class StubAccessor
    {
        public int SuccessfulProperty { get; set; }

        public int FailingProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { throw new InvalidOperationException(); }
        }
    }
}
