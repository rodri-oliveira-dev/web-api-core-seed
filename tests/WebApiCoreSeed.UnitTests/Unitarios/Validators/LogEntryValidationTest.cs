using FluentValidation.TestHelper;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Validators
{
    public class LogEntryValidationTest
    {
        [Fact(DisplayName = "LogEntry falha validacao quando mensagem ausente")]
        [Trait("Validators", "LogEntry")]
        public void LogEntryQuandoMensagemAusenteDeveFalharValidacao()
        {
            var logEntry = new LogEntry { Message = string.Empty };
            var validator = new LogEntryValidation();

            var result = validator.TestValidate(logEntry);

            result.ShouldHaveValidationErrorFor(log => log.Message);
        }

        [Fact(DisplayName = "LogEntry passa validacao quando campos obrigatorios existem")]
        [Trait("Validators", "LogEntry")]
        public void LogEntryQuandoCamposObrigatoriosValidosDevePassarValidacao()
        {
            var logEntry = new LogEntry { Message = "Evento registrado" };
            var validator = new LogEntryValidation();

            var result = validator.TestValidate(logEntry);

            result.ShouldNotHaveValidationErrorFor(log => log.Message);
            result.ShouldNotHaveValidationErrorFor(log => log.LogLevel);
        }
    }
}
