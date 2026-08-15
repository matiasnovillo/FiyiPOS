namespace FiyiPOS.Mailing
{
    public interface IEmail
    {
        Task SendSignUpEmailAsync(string toEmail, string fullName, string activationUrl);

        Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetUrl);

        Task SendLowStockAlertAsync(
            string toEmail,
            string toName,
            string storeName,
            DateTime saleDate,
            List<(string Name, decimal Stock, decimal MinStock)> products);

        Task SendErrorAlertAsync(string message, string source, string stackTrace, DateTime occurredAt);

        Task SendNewUserAlertAsync(string userName, string userEmail, string role, DateTime registeredAt);

        Task SendCashCutDiscrepancyAlertAsync(
            string toEmail, string toName,
            string storeName, string operatorName,
            decimal difference, decimal expectedAmount, decimal actualAmount,
            DateTime closedAt);
    }
}
