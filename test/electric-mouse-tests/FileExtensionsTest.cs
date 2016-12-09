using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Services;
using Xunit;

namespace electric_mouse_tests
{
    public class FileExtensionsTest
    {
        [Fact]
        public void HasExtension_CheckForPNGInFileNameWithPNG_ReturnsTrue()
        {
            // arrange
            string fileName = "name.png";
            string extensionToCheck = ".png";

            // act
            bool isPngImage = fileName.HasExtension(extensionToCheck);
            
            // assert
            Assert.True(isPngImage);
        }

        [Fact]
        public void HasExtension_CheckForPNGInFileNameWithNoPNG_ReturnsTrue()
        {
            // arrange
            string fileName = "name.jpg";
            string extensionToCheck = ".png";

            // act
            bool isPngImage = fileName.HasExtension(extensionToCheck);

            // assert
            Assert.False(isPngImage);
        }

        [Fact]
        public void ConvertMegabytesToBytes_Insert5Megabytes_Return5242880()
        {
            // arrange
            long megabytes = 5;
            long expectedBytes = 5242880L;

            // act
            long resultBytes = megabytes.ConvertMegabytesToBytes();

            // assert
            Assert.Equal(resultBytes, expectedBytes);
        }
    }
}
