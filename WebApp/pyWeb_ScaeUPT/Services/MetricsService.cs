using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;

namespace pyWeb_ScaeUPT.Services
{
    public interface IMetricsService
    {
        void TrackUserLogin(string userId, string email, string loginMethod, string semester);
        void TrackLoginFailure(string reason, string loginMethod);
        void TrackQRGeneration(string userId, string dni, string matricula, bool isRegeneration, long generationTime);
        void TrackQRGenerationFailure(string userId, string error);
        void TrackUserInfoView(string userId, string email, string matricula);
        void TrackUserHistoryView(string userId, string dni, int recordCount);
        void TrackUserLogout(string userId, string email, TimeSpan sessionDuration);
        void TrackCustomEvent(string eventName, Dictionary<string, string> properties = null);
        void TrackCustomMetric(string metricName, double value, Dictionary<string, string> properties = null);
        void TrackPerformance(string operation, long durationMs);
    }

    public class MetricsService : IMetricsService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<MetricsService> _logger;

        public MetricsService(TelemetryClient telemetryClient, ILogger<MetricsService> logger)
        {
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        public void TrackUserLogin(string userId, string email, string loginMethod, string semester)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["Email"] = email,
                    ["LoginMethod"] = loginMethod,
                    ["Domain"] = email.Split('@').LastOrDefault() ?? "unknown",
                    ["Semester"] = semester,
                    ["Hour"] = DateTime.Now.Hour.ToString(),
                    ["DayOfWeek"] = DateTime.Now.DayOfWeek.ToString(),
                    ["IsWeekend"] = (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday).ToString()
                };

                _telemetryClient.TrackEvent("UserLogin", properties);
                _telemetryClient.TrackMetric("LoginSuccessCount", 1, properties);
                
                _logger.LogInformation($"Tracked login for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user login");
            }
        }

        public void TrackLoginFailure(string reason, string loginMethod)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["Reason"] = reason,
                    ["LoginMethod"] = loginMethod,
                    ["Hour"] = DateTime.Now.Hour.ToString()
                };

                _telemetryClient.TrackEvent("LoginFailed", properties);
                _telemetryClient.TrackMetric("LoginFailureCount", 1, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking login failure");
            }
        }

        public void TrackQRGeneration(string userId, string dni, string matricula, bool isRegeneration, long generationTime)
        {
            try
            {
                var eventName = isRegeneration ? "QRCodeRegenerated" : "QRCodeFirstGenerated";
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["DNI"] = dni,
                    ["Matricula"] = matricula,
                    ["IsRegeneration"] = isRegeneration.ToString(),
                    ["GenerationTime"] = generationTime.ToString(),
                    ["Hour"] = DateTime.Now.Hour.ToString(),
                    ["DayOfWeek"] = DateTime.Now.DayOfWeek.ToString()
                };

                _telemetryClient.TrackEvent(eventName, properties);
                _telemetryClient.TrackMetric("QRGenerationTime", generationTime, properties);
                
                if (isRegeneration)
                {
                    _telemetryClient.TrackMetric("QRRegenerationCount", 1, properties);
                }
                else
                {
                    _telemetryClient.TrackMetric("QRFirstGenerationCount", 1, properties);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking QR generation");
            }
        }

        public void TrackQRGenerationFailure(string userId, string error)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["Error"] = error
                };

                _telemetryClient.TrackEvent("QRGenerationFailed", properties);
                _telemetryClient.TrackMetric("QRGenerationFailureCount", 1, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking QR generation failure");
            }
        }

        public void TrackUserInfoView(string userId, string email, string matricula)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["Email"] = email,
                    ["Matricula"] = matricula,
                    ["Hour"] = DateTime.Now.Hour.ToString()
                };

                _telemetryClient.TrackEvent("UserInfoViewed", properties);
                _telemetryClient.TrackMetric("UserInfoViewCount", 1, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user info view");
            }
        }

        public void TrackUserHistoryView(string userId, string dni, int recordCount)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["DNI"] = dni,
                    ["HistoryCount"] = recordCount.ToString(),
                    ["Hour"] = DateTime.Now.Hour.ToString(),
                    ["RecordsFound"] = recordCount.ToString()
                };

                _telemetryClient.TrackEvent("UserHistoryViewed", properties);
                _telemetryClient.TrackMetric("HistoryViewCount", 1, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking history view");
            }
        }

        public void TrackUserLogout(string userId, string email, TimeSpan sessionDuration)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["UserId"] = userId,
                    ["Email"] = email,
                    ["SessionDurationMinutes"] = sessionDuration.TotalMinutes.ToString("F2"),
                    ["Hour"] = DateTime.Now.Hour.ToString()
                };

                _telemetryClient.TrackEvent("UserLogout", properties);
                _telemetryClient.TrackMetric("LogoutCount", 1, properties);
                _telemetryClient.TrackMetric("SessionDuration", sessionDuration.TotalMinutes, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user logout");
            }
        }

        public void TrackCustomEvent(string eventName, Dictionary<string, string> properties = null)
        {
            try
            {
                _telemetryClient.TrackEvent(eventName, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking custom event {eventName}");
            }
        }

        public void TrackCustomMetric(string metricName, double value, Dictionary<string, string> properties = null)
        {
            try
            {
                _telemetryClient.TrackMetric(metricName, value, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking custom metric {metricName}");
            }
        }

        public void TrackPerformance(string operation, long durationMs)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["Operation"] = operation,
                    ["Hour"] = DateTime.Now.Hour.ToString()
                };

                _telemetryClient.TrackMetric($"{operation}Duration", durationMs, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking performance for {operation}");
            }
        }
    }
}