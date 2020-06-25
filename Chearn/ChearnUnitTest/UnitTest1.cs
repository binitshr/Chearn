using Chearn.Controllers;
using Chearn.Models;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChearnUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CanbeEnrolledby_UserIsAdmin_ReturnsTrue()
        {
            //Arrange
            var enrollment = new Enrollment();

            //Act 
            var result = enrollment.CanBeEnrolledBy(new User { IsAdmin = false });

            //Assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void EnsureCorrectCount() 
        {
            Assert.AreEqual(ReviewController.ComputeNextReviewTime(new Review() { Level = 1}), 1.8);
            Assert.AreEqual(ReviewController.ComputeNextReviewTime(new Review() { Level = 2 }), 3.24);
        }
    }
}
