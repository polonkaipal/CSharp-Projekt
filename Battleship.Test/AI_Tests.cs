using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Battleship.Test
{
    [TestClass]
    public class AI_Tests
    {
        [DataRow(0, 10)]
        [DataRow(10, 0)]
        [DataTestMethod]
        public void IsCellWall_ReturnTrue(int X, int Y)
        {
            //Arrange + Act + Assert
            Assert.IsTrue(AiMethods.isCellWall(X, Y));
        }

        [DataRow(5, 2)]
        [DataRow(6, 8)]
        [DataTestMethod]
        public void IsCellWall_ReturnFalse(int X, int Y)
        {
            //Arrange + Act + Assert
            Assert.IsFalse(AiMethods.isCellWall(X, Y));
        }

        static char[,] playerPlayfield = new char[10, 10];

        [DataRow(2, 2)]
        [DataRow(4, 5)]
        [DataTestMethod]
        public void IsCellShootedAI_ReturnTrue(int Y, int X)
        {
            //Arrange
            playerPlayfield[2, 2] = 'T';
            playerPlayfield[4, 5] = 'V';

            //Act + Assert
            Assert.IsTrue(AiMethods.isCellShootedAI(X, Y, playerPlayfield));
        }

        [DataRow(6, 8)]
        [DataRow(2, 4)]
        [DataTestMethod]
        public void IsCellShootedAI_ReturnFalse(int Y, int X)
        {
            //Arrange
            playerPlayfield[6, 8] = '5';
            playerPlayfield[2, 4] = '\0';

            //Act + Assert
            Assert.IsFalse(AiMethods.isCellShootedAI(X, Y, playerPlayfield));
        }

        [DataRow(6, 8)]
        [DataRow(2, 4)]
        [DataTestMethod]
        public void IsHitPlayerShipUnit_ReturnTrue(int Y, int X)
        {
            //Arrange
            playerPlayfield[6, 8] = '5';
            playerPlayfield[2, 4] = '1';

            //Act + Assert
            Assert.IsTrue(AiMethods.isHitPlayerShipUnit(X, Y, playerPlayfield));
        }

        [DataRow(6, 8)]
        [DataRow(2, 4)]
        [DataTestMethod]
        public void IsHitPlayerShipUnit_ReturnFalse(int Y, int X)
        {
            //Arrange
            playerPlayfield[6, 8] = '\0';
            playerPlayfield[2, 4] = 'T';

            //Act + Assert
            Assert.IsFalse(AiMethods.isHitPlayerShipUnit(X, Y, playerPlayfield));
        }
    }
}
