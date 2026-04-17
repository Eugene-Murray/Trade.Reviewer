namespace Trade.Dashboard.Client.Services;

public sealed class ApiException(string message) : Exception(message);
