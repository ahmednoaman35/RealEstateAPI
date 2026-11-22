namespace RealEstateAPI.Services
{
    public interface IWhatsAppService
    {
        string GenerateLink(string whatsAppNumber, string message);
    }

    public class WhatsAppService : IWhatsAppService
    {
        public string GenerateLink(string phoneNumber, string message)
        {
            // حذف أي مسافات أو رموز
            phoneNumber = phoneNumber.Replace(" ", "")
                                     .Replace("+", "")
                                     .Replace("-", "");

            // تحويل الرقم المصري للصيغة الدولية
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = "2" + phoneNumber;
            }

            var encodedMessage = Uri.EscapeDataString(message);

            return $"https://wa.me/{phoneNumber}?text={encodedMessage}";
        }
    }
}