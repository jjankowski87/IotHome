using System.Threading.Tasks;
using FluentAssertions;
using IotHomeDevice.Implementation.Sensor;
using IotHomeDevice.Interface;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace IotHomeDevice.Implementation.Test.Sensor
{
    public class DS18B20ThermometerTests
    {
        [Fact]
        public void ShouldReturnTemperature_WhenCommandIsExecutedSuccessfully()
        {
            // given
            const string content = "4b 01 4b 46 7f ff 05 10 e1 : crc=e1 YES\n" +
                                   "4b 01 4b 46 7f ff 05 10 e1 t=20687";

            var mocker = new AutoMocker();
            mocker.Setup<IShellHelper, Task<string>>(h => h.ExecuteCommandAsync(It.IsAny<string>()))
                .ReturnsAsync(content);

            var sensor = mocker.CreateInstance<DS18B20Thermometer>();

            // when
            var temperature = sensor.ReadValue();

            // then
            temperature.Should().Be(20.7m);
        }
    }
}
