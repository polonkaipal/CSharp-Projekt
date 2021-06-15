using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Battleship.Test
{
    [TestClass]
    public class AI_Tests
    {   [DataRow(false,false,false,false)]
        [DataRow(false,true,false,false)]
        [DataRow(true,true,false,true)]
        [DataTestMethod]
        public void ShipDestroyed_AllDirectionShooted_ShouldReturnFalse(bool up, bool down, bool left, bool right)
        {
            var mainWindow = new MainWindow(); 
            //Arrange + Act + Assert
            Assert.IsFalse(mainWindow.shipDestroyed());
        }


        [DataRow(0,10)]
        [DataRow(10,0)]
        [DataTestMethod]
        public void IsCellWall_ReturnFalse(int X, int Y)
        {
            //Arrange + Act + Assert
            Assert.IsTrue(isCellWall(X, Y));
        }
    }
}
