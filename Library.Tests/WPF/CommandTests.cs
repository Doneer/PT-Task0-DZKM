using Library.WPF.Commands;

namespace Library.Tests.WPF
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void RelayCommand_Execute_CallsAction()
        {
            bool actionCalled = false;
            var command = new RelayCommand(() => actionCalled = true);

            command.Execute(null);

            Assert.IsTrue(actionCalled);
        }

        [TestMethod]
        public void RelayCommand_CanExecute_ReturnsTrue_WhenNoCanExecuteProvided()
        {
            var command = new RelayCommand(() => { });

            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        public void RelayCommand_CanExecute_ReturnsCanExecuteResult()
        {
            bool canExecute = false;
            var command = new RelayCommand(() => { }, () => canExecute);

            Assert.IsFalse(command.CanExecute(null));

            canExecute = true;
            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        public void RelayCommand_WithParameter_Execute_CallsActionWithParameter()
        {
            string receivedParameter = null;
            var command = new RelayCommand(param => receivedParameter = param as string);

            command.Execute("test parameter");

            Assert.AreEqual("test parameter", receivedParameter);
        }

        [TestMethod]
        public async Task AsyncRelayCommand_Execute_CallsAsyncAction()
        {
            bool actionCalled = false;
            var command = new AsyncRelayCommand(async () =>
            {
                await Task.Delay(10);
                actionCalled = true;
            });

            command.Execute(null);
            await Task.Delay(50); 

            Assert.IsTrue(actionCalled);
        }

        [TestMethod]
        public void AsyncRelayCommand_CanExecute_ReturnsFalse_WhenExecuting()
        {
            var command = new AsyncRelayCommand(async () => await Task.Delay(100));

            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);

            Assert.IsFalse(command.CanExecute(null));
        }
    }
}