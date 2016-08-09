using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Control;
using Moq;

namespace Tiles.Gsm.Tests
{
    [TestClass]
    public class GameScreenTests
    {
        // GameScreen is used via inheritance
        class TestScreen : GameScreen
        {
            public TestScreen(bool blockForInput = false)
            {
                BlockForInput = blockForInput;
            }

            public override void Draw()
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }

            public override void OnKeyPress(KeyPressEventArgs args)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void ClassInvariants()
        {
            var screen = new TestScreen();

            Assert.IsFalse(screen.PropagateUpdate);
            Assert.IsFalse(screen.PropagateInput);
            Assert.IsTrue(screen.PropagateDraw);

            Assert.AreEqual(ScreenState.None, screen.State);
            Assert.IsNull(screen.ScreenManager);
        }

        [TestMethod]
        public void OnEnter()
        {
            var screenManagerMock = new Mock<IGameScreenManager>();
            var screen = new TestScreen();

            screen.OnEnter(screenManagerMock.Object);

            Assert.AreEqual(ScreenState.Active, screen.State);
            Assert.AreSame(screenManagerMock.Object, screen.ScreenManager);
            screenManagerMock.Verify(x => x.Remove(It.IsAny<IGameScreen>()), Times.Never());
        }

        [TestMethod]
        public void OnExit()
        {
            var screenManagerMock = new Mock<IGameScreenManager>();
            var screen = new TestScreen();

            Assert.AreEqual(ScreenState.None, screen.State);
            Assert.IsNull(screen.ScreenManager);

            screen.OnExit();

            Assert.AreEqual(ScreenState.None, screen.State);
            Assert.IsNull(screen.ScreenManager);

            screen.OnEnter(screenManagerMock.Object);

            Assert.AreEqual(ScreenState.Active, screen.State);
            Assert.AreSame(screenManagerMock.Object, screen.ScreenManager);

            screen.OnExit();
            Assert.AreEqual(ScreenState.None, screen.State);
            Assert.IsNull(screen.ScreenManager);

            screenManagerMock.Verify(x => x.Remove(It.IsAny<IGameScreen>()), Times.Never());
        }

        [TestMethod]
        public void Exit()
        {
            var screenManagerMock = new Mock<IGameScreenManager>();
            var screen = new TestScreen();

            screen.OnEnter(screenManagerMock.Object);
            Assert.AreEqual(ScreenState.Active, screen.State);
            Assert.AreSame(screenManagerMock.Object, screen.ScreenManager);

            screen.Exit();

            screenManagerMock.Verify(x => x.Remove(It.IsAny<IGameScreen>()), Times.Once());
        }

        [TestMethod]
        public void BlocksForInput()
        {
            var blockerScreen = new TestScreen(true);
            var nonBlockerScreen = new TestScreen(false);

            Assert.IsTrue(blockerScreen.BlockForInput);
            Assert.IsFalse(nonBlockerScreen.BlockForInput);
        }

        [TestMethod]
        public void Load()
        {
            var screen = new TestScreen();
            screen.Load();
        }

        [TestMethod]
        public void Unload()
        {
            var screen = new TestScreen();
            screen.Unload();
        }
    }
}
