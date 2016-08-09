using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Control;
using Moq;
using System.Collections.Generic;


namespace Tiles.Gsm.Tests
{
    [TestClass]
    public class GameScreenManagerTests
    {
        [TestMethod]
        public void Add()
        {
            var screenMock = new Mock<IGameScreen>();
            var manager = new GameScreenManager();

            manager.Add(screenMock.Object);

            screenMock.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock.Verify(x => x.Load(), Times.Once());

            screenMock.Verify(x => x.OnExit(), Times.Never());
            screenMock.Verify(x => x.Unload(), Times.Never());
        }

        [TestMethod]
        public void BlockForInput_NoBlockers()
        {
            var screens = new List<IGameScreen>{
                MockScreen(blockForInput: false).Object,
                MockScreen(blockForInput: false).Object,
                MockScreen(blockForInput: false).Object,
                MockScreen(blockForInput: false).Object
            };

            var manager = new GameScreenManager();
            foreach (var screen in screens)
            {
                manager.Add(screen);
            }

            Assert.IsFalse(manager.BlockForInput);
        }

        [TestMethod]
        public void BlockForInput_HasBlocker()
        {
            var screens = new List<IGameScreen>{
                MockScreen(blockForInput: false).Object,
                MockScreen(blockForInput: true).Object,
                MockScreen(blockForInput: false).Object,
                MockScreen(blockForInput: false).Object
            };

            var manager = new GameScreenManager();
            foreach (var screen in screens)
            {
                manager.Add(screen);
            }

            Assert.IsTrue(manager.BlockForInput);
        }

        [TestMethod]
        public void Remove()
        {
            var screenMock = new Mock<IGameScreen>();
            var manager = new GameScreenManager();

            manager.Add(screenMock.Object);
            manager.Remove(screenMock.Object);

            screenMock.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock.Verify(x => x.Load(), Times.Once());

            screenMock.Verify(x => x.OnExit(), Times.Once());
            screenMock.Verify(x => x.Unload(), Times.Once());
        }

        [TestMethod]
        public void SingleScreen()
        {
            var screenMock = new Mock<IGameScreen>();
            var manager = new GameScreenManager();

            manager.Add(screenMock.Object);

            screenMock.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock.Verify(x => x.Load(), Times.Once());

            manager.Draw();
            screenMock.Verify(x => x.Draw(), Times.Once());

            manager.Update();
            screenMock.Verify(x => x.Update(), Times.Once());

            var keyPress = KeyPress();

            screenMock.Setup(x => x.State).Returns(ScreenState.None);
            manager.OnKeyPress(null, keyPress);
            screenMock.Verify(x => x.OnKeyPress(keyPress), Times.Never());

            screenMock.Setup(x => x.State).Returns(ScreenState.Active);
            manager.OnKeyPress(null, keyPress);
            screenMock.Verify(x => x.OnKeyPress(keyPress), Times.Once());
        }

        [TestMethod]
        public void MixedPropagation()
        {
            var manager = new GameScreenManager();

            var screenMock1 = MockScreen();
            manager.Add(screenMock1.Object);

            var screenMock2 = MockScreen(propagateDraw: false, propagateUpdate: false, propagateInput: false);
            manager.Add(screenMock2.Object);

            var screenMock3 = MockScreen(propagateDraw: false, propagateUpdate: true, propagateInput: false, state: ScreenState.Active);
            manager.Add(screenMock3.Object);

            var screenMock4 = MockScreen(propagateDraw: false, propagateUpdate: true, propagateInput: false, state: ScreenState.None);
            manager.Add(screenMock4.Object);

            var screenMock5 = MockScreen(propagateDraw: false, propagateInput: true, propagateUpdate: true);
            manager.Add(screenMock5.Object);

            var screenMock6 = MockScreen(propagateDraw: true, propagateInput: true, propagateUpdate: true);
            manager.Add(screenMock6.Object);

            manager.Draw();

            screenMock1.Verify(x => x.Draw(), Times.Never());
            screenMock2.Verify(x => x.Draw(), Times.Never());
            screenMock3.Verify(x => x.Draw(), Times.Never());
            screenMock4.Verify(x => x.Draw(), Times.Never());
            screenMock5.Verify(x => x.Draw(), Times.Once());
            screenMock6.Verify(x => x.Draw(), Times.Once());


            manager.Update();

            screenMock1.Verify(x => x.Update(), Times.Never());
            screenMock2.Verify(x => x.Update(), Times.Once());
            screenMock3.Verify(x => x.Update(), Times.Once());
            screenMock4.Verify(x => x.Update(), Times.Once());
            screenMock5.Verify(x => x.Update(), Times.Once());
            screenMock6.Verify(x => x.Update(), Times.Once());


            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);

            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Never());
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Never());
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Never());
            screenMock5.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock6.Verify(x => x.OnKeyPress(keyPress), Times.Once());
        }

        [TestMethod]
        public void PropagateAll()
        {
            var manager = new GameScreenManager();

            var screenMock1 = MockScreen();
            manager.Add(screenMock1.Object);

            var screenMock2 = MockScreen();
            manager.Add(screenMock2.Object);

            var screenMock3 = MockScreen(state: ScreenState.None);
            manager.Add(screenMock3.Object);


            manager.Draw();

            screenMock1.Verify(x => x.Draw(), Times.Once());
            screenMock2.Verify(x => x.Draw(), Times.Once());
            screenMock3.Verify(x => x.Draw(), Times.Once());


            manager.Update();

            screenMock1.Verify(x => x.Update(), Times.Once());
            screenMock2.Verify(x => x.Update(), Times.Once());
            screenMock3.Verify(x => x.Update(), Times.Once());


            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);

            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Never());
        }

        [TestMethod]
        public void ScreenLoad_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();

            var manager = new GameScreenManager();

            screenMock2.Setup(x => x.Load()).Callback(() =>
            {
                manager.Remove(screenMock1.Object);
                manager.Add(screenMock3.Object);
            });

            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);

            manager.Update();
            screenMock1.Verify(x => x.Unload(), Times.Once());
            screenMock1.Verify(x => x.OnExit(), Times.Once());

            screenMock3.Verify(x => x.Load(), Times.Once());
            screenMock3.Verify(x => x.OnEnter(manager), Times.Once());

            screenMock1.Verify(x => x.Update(), Times.Exactly(0));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(1));


            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(0));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(1));

            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(0));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
        }

        [TestMethod]
        public void ScreenUnload_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();
            var screenMock4 = MockScreen();

            var manager = new GameScreenManager();

            screenMock2.Setup(x => x.Unload()).Callback(() =>
            {
                manager.Remove(screenMock1.Object);
                manager.Add(screenMock3.Object);
            });

            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);
            manager.Add(screenMock4.Object);

            screenMock1.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock1.Verify(x => x.Load(), Times.Once());
            screenMock1.Verify(x => x.Unload(), Times.Never());
            screenMock1.Verify(x => x.OnExit(), Times.Never());

            screenMock2.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock2.Verify(x => x.Load(), Times.Once);
            screenMock2.Verify(x => x.Unload(), Times.Never());
            screenMock2.Verify(x => x.OnExit(), Times.Never());

            screenMock3.Verify(x => x.OnEnter(manager), Times.Never());
            screenMock3.Verify(x => x.Load(), Times.Never());
            screenMock3.Verify(x => x.Unload(), Times.Never());
            screenMock3.Verify(x => x.OnExit(), Times.Never());

            screenMock4.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock4.Verify(x => x.Load(), Times.Once());
            screenMock4.Verify(x => x.Unload(), Times.Never());
            screenMock4.Verify(x => x.OnExit(), Times.Never());

            manager.Update();
            screenMock1.Verify(x => x.Update(), Times.Exactly(1));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(0));
            screenMock4.Verify(x => x.Update(), Times.Exactly(1));

            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(0));
            screenMock4.Verify(x => x.Draw(), Times.Exactly(1));

            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(0));
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));


            manager.Remove(screenMock2.Object);

            screenMock1.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock1.Verify(x => x.Load(), Times.Once());
            screenMock1.Verify(x => x.Unload(), Times.Once());
            screenMock1.Verify(x => x.OnExit(), Times.Once());

            screenMock2.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock2.Verify(x => x.Load(), Times.Once);
            screenMock2.Verify(x => x.Unload(), Times.Once());
            screenMock2.Verify(x => x.OnExit(), Times.Once());

            screenMock3.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock3.Verify(x => x.Load(), Times.Once());
            screenMock3.Verify(x => x.Unload(), Times.Never());
            screenMock3.Verify(x => x.OnExit(), Times.Never());

            screenMock4.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock4.Verify(x => x.Load(), Times.Once());
            screenMock4.Verify(x => x.Unload(), Times.Never());
            screenMock4.Verify(x => x.OnExit(), Times.Never());


            manager.Update();
            screenMock1.Verify(x => x.Update(), Times.Exactly(1));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(1));
            screenMock4.Verify(x => x.Update(), Times.Exactly(2));

            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock4.Verify(x => x.Draw(), Times.Exactly(2));

            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(2));
        }

        [TestMethod]
        public void ScreenOnEnter_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();

            var manager = new GameScreenManager();

            screenMock2.Setup(x => x.OnEnter(manager)).Callback(() =>
            {
                manager.Remove(screenMock1.Object);
                manager.Add(screenMock3.Object);
            });

            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);

            manager.Update();
            screenMock1.Verify(x => x.Unload(), Times.Once());
            screenMock1.Verify(x => x.OnExit(), Times.Once());

            screenMock3.Verify(x => x.Load(), Times.Once());
            screenMock3.Verify(x => x.OnEnter(manager), Times.Once());

            screenMock1.Verify(x => x.Update(), Times.Exactly(0));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(1));


            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(0));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(1));

            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(0));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
        }

        [TestMethod]
        public void ScreenOnExit_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();
            var screenMock4 = MockScreen();

            var manager = new GameScreenManager();

            screenMock2.Setup(x => x.OnExit()).Callback(() =>
            {
                manager.Remove(screenMock1.Object);
                manager.Add(screenMock3.Object);
            });

            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);
            manager.Add(screenMock4.Object);

            screenMock1.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock1.Verify(x => x.Load(), Times.Once());
            screenMock1.Verify(x => x.Unload(), Times.Never());
            screenMock1.Verify(x => x.OnExit(), Times.Never());

            screenMock2.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock2.Verify(x => x.Load(), Times.Once);
            screenMock2.Verify(x => x.Unload(), Times.Never());
            screenMock2.Verify(x => x.OnExit(), Times.Never());

            screenMock3.Verify(x => x.OnEnter(manager), Times.Never());
            screenMock3.Verify(x => x.Load(), Times.Never());
            screenMock3.Verify(x => x.Unload(), Times.Never());
            screenMock3.Verify(x => x.OnExit(), Times.Never());

            screenMock4.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock4.Verify(x => x.Load(), Times.Once());
            screenMock4.Verify(x => x.Unload(), Times.Never());
            screenMock4.Verify(x => x.OnExit(), Times.Never());

            manager.Update();
            screenMock1.Verify(x => x.Update(), Times.Exactly(1));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(0));
            screenMock4.Verify(x => x.Update(), Times.Exactly(1));

            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(0));
            screenMock4.Verify(x => x.Draw(), Times.Exactly(1));

            var keyPress = KeyPress();
            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(0));
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));


            manager.Remove(screenMock2.Object);

            screenMock1.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock1.Verify(x => x.Load(), Times.Once());
            screenMock1.Verify(x => x.Unload(), Times.Once());
            screenMock1.Verify(x => x.OnExit(), Times.Once());

            screenMock2.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock2.Verify(x => x.Load(), Times.Once);
            screenMock2.Verify(x => x.Unload(), Times.Once());
            screenMock2.Verify(x => x.OnExit(), Times.Once());

            screenMock3.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock3.Verify(x => x.Load(), Times.Once());
            screenMock3.Verify(x => x.Unload(), Times.Never());
            screenMock3.Verify(x => x.OnExit(), Times.Never());

            screenMock4.Verify(x => x.OnEnter(manager), Times.Once());
            screenMock4.Verify(x => x.Load(), Times.Once());
            screenMock4.Verify(x => x.Unload(), Times.Never());
            screenMock4.Verify(x => x.OnExit(), Times.Never());


            manager.Update();
            screenMock1.Verify(x => x.Update(), Times.Exactly(1));
            screenMock2.Verify(x => x.Update(), Times.Exactly(1));
            screenMock3.Verify(x => x.Update(), Times.Exactly(1));
            screenMock4.Verify(x => x.Update(), Times.Exactly(2));

            manager.Draw();
            screenMock1.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock3.Verify(x => x.Draw(), Times.Exactly(1));
            screenMock4.Verify(x => x.Draw(), Times.Exactly(2));

            manager.OnKeyPress(null, keyPress);
            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(1));
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(2));
        }

        [TestMethod]
        public void Update_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();
            var screenMock4 = MockScreen();

            var manager = new GameScreenManager();
            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);
            manager.Add(screenMock3.Object);

            screenMock2.Setup(x => x.Update()).Callback(() =>
            {
                manager.Add(screenMock4.Object);
                manager.Remove(screenMock3.Object);
            });

            manager.Update();

            screenMock1.Verify(x => x.Update(), Times.Once());
            screenMock2.Verify(x => x.Update(), Times.Once());
            screenMock3.Verify(x => x.Update(), Times.Once());
            screenMock4.Verify(x => x.Update(), Times.Never());

            screenMock2.Setup(x => x.Update()).Callback(() => { });
            manager.Update();

            screenMock1.Verify(x => x.Update(), Times.Exactly(2));
            screenMock2.Verify(x => x.Update(), Times.Exactly(2));
            screenMock3.Verify(x => x.Update(), Times.Once());
            screenMock4.Verify(x => x.Update(), Times.Once());


            screenMock1.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock1.Verify(x => x.Load(), Times.Exactly(1));
            screenMock1.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock1.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock2.Verify(x => x.Load(), Times.Exactly(1));
            screenMock2.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock2.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock3.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock3.Verify(x => x.Load(), Times.Exactly(1));
            screenMock3.Verify(x => x.Unload(), Times.Exactly(1));
            screenMock3.Verify(x => x.OnExit(), Times.Exactly(1));

            screenMock4.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock4.Verify(x => x.Load(), Times.Exactly(1));
            screenMock4.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock4.Verify(x => x.OnExit(), Times.Exactly(0));
        }

        [TestMethod]
        public void Draw_ScreenChanges()
        {
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();
            var screenMock4 = MockScreen();

            var manager = new GameScreenManager();
            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);
            manager.Add(screenMock3.Object);

            screenMock1.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock1.Verify(x => x.Load(), Times.Exactly(1));
            screenMock1.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock1.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock2.Verify(x => x.Load(), Times.Exactly(1));
            screenMock2.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock2.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock3.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock3.Verify(x => x.Load(), Times.Exactly(1));
            screenMock3.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock3.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock4.Verify(x => x.OnEnter(manager), Times.Exactly(0));
            screenMock4.Verify(x => x.Load(), Times.Exactly(0));
            screenMock4.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock4.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Setup(x => x.Draw()).Callback(() =>
            {
                manager.Add(screenMock4.Object);
                manager.Remove(screenMock3.Object);
            });

            manager.Draw();

            screenMock1.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock1.Verify(x => x.Load(), Times.Exactly(1));
            screenMock1.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock1.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock2.Verify(x => x.Load(), Times.Exactly(1));
            screenMock2.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock2.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock3.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock3.Verify(x => x.Load(), Times.Exactly(1));
            screenMock3.Verify(x => x.Unload(), Times.Exactly(1));
            screenMock3.Verify(x => x.OnExit(), Times.Exactly(1));

            screenMock4.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock4.Verify(x => x.Load(), Times.Exactly(1));
            screenMock4.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock4.Verify(x => x.OnExit(), Times.Exactly(0));


            screenMock1.Verify(x => x.Draw(), Times.Once());
            screenMock2.Verify(x => x.Draw(), Times.Once());
            screenMock3.Verify(x => x.Draw(), Times.Never());
            screenMock4.Verify(x => x.Draw(), Times.Once());

            screenMock2.Setup(x => x.Draw()).Callback(() => { });
            manager.Draw();

            screenMock1.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock1.Verify(x => x.Load(), Times.Exactly(1));
            screenMock1.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock1.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock2.Verify(x => x.Load(), Times.Exactly(1));
            screenMock2.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock2.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock3.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock3.Verify(x => x.Load(), Times.Exactly(1));
            screenMock3.Verify(x => x.Unload(), Times.Exactly(1));
            screenMock3.Verify(x => x.OnExit(), Times.Exactly(1));

            screenMock4.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock4.Verify(x => x.Load(), Times.Exactly(1));
            screenMock4.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock4.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock1.Verify(x => x.Draw(), Times.Exactly(2));
            screenMock2.Verify(x => x.Draw(), Times.Exactly(2));
            screenMock3.Verify(x => x.Draw(), Times.Never());
            screenMock4.Verify(x => x.Draw(), Times.Exactly(2));
        }


        [TestMethod]
        public void OnKeyPress_ScreenChanges()
        {
            var keyPress = KeyPress();
            var screenMock1 = MockScreen();
            var screenMock2 = MockScreen();
            var screenMock3 = MockScreen();
            var screenMock4 = MockScreen();

            var manager = new GameScreenManager();
            manager.Add(screenMock1.Object);
            manager.Add(screenMock2.Object);
            manager.Add(screenMock3.Object);

            screenMock2.Setup(x => x.OnKeyPress(keyPress)).Callback(() =>
            {
                manager.Add(screenMock4.Object);
                manager.Remove(screenMock3.Object);
            });

            manager.OnKeyPress(null, keyPress);

            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Never());

            screenMock2.Setup(x => x.OnKeyPress(keyPress)).Callback(() => { });
            manager.OnKeyPress(null, keyPress);

            screenMock1.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(2));
            screenMock2.Verify(x => x.OnKeyPress(keyPress), Times.Exactly(2));
            screenMock3.Verify(x => x.OnKeyPress(keyPress), Times.Once());
            screenMock4.Verify(x => x.OnKeyPress(keyPress), Times.Once());


            screenMock1.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock1.Verify(x => x.Load(), Times.Exactly(1));
            screenMock1.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock1.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock2.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock2.Verify(x => x.Load(), Times.Exactly(1));
            screenMock2.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock2.Verify(x => x.OnExit(), Times.Exactly(0));

            screenMock3.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock3.Verify(x => x.Load(), Times.Exactly(1));
            screenMock3.Verify(x => x.Unload(), Times.Exactly(1));
            screenMock3.Verify(x => x.OnExit(), Times.Exactly(1));

            screenMock4.Verify(x => x.OnEnter(manager), Times.Exactly(1));
            screenMock4.Verify(x => x.Load(), Times.Exactly(1));
            screenMock4.Verify(x => x.Unload(), Times.Exactly(0));
            screenMock4.Verify(x => x.OnExit(), Times.Exactly(0));
        }

        Mock<IGameScreen> MockScreen(ScreenState state = ScreenState.Active, bool propagateDraw = true, bool propagateUpdate = true, bool propagateInput = true, bool blockForInput = false)
        {
            var screenMock = new Mock<IGameScreen>();;
            screenMock.Setup(x => x.State).Returns(state);
            screenMock.Setup(x => x.PropagateDraw).Returns(propagateDraw);
            screenMock.Setup(x => x.PropagateUpdate).Returns(propagateUpdate);
            screenMock.Setup(x => x.PropagateInput).Returns(propagateInput);
            screenMock.Setup(x => x.BlockForInput).Returns(blockForInput);
            return screenMock;
        }

        KeyPressEventArgs KeyPress()
        {
            return new KeyPressEventArgs(ConsoleKey.A, 'a', false, false, false);
        }
    }
}
