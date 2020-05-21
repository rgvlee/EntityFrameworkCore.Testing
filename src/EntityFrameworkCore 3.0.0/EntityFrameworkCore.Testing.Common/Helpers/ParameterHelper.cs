using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     A helper for parameter matching.
    /// </summary>
    public class ParameterMatchingHelper
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(ParameterMatchingHelper));

        /// <summary>
        ///     Determines whether the invocation parameters match the set up parameters.
        /// </summary>
        /// <param name="setUpParameters">The set up parameters.</param>
        /// <param name="invocationParameters">The invocation parameters.</param>
        /// <returns>true the invocation parameters are a partial or full match of the set up parameters.</returns>
        /// <remarks>
        ///     If the parameters are DbParameters, parameter name and value are compared.
        ///     Parameter name matching is case insensitive.
        ///     If the value is a string, the matching is case insensitive.
        ///     For everything else an exact match is required.
        /// </remarks>
        public static bool DoInvocationParametersMatchSetUpParameters(IEnumerable<object> setUpParameters, IEnumerable<object> invocationParameters)
        {
            var setUpParametersAsList = setUpParameters.ToList();
            var invocationParametersAsList = invocationParameters.ToList();

            var matches = new Dictionary<int, int>();
            for (var i = 0; i < invocationParametersAsList.Count; i++)
            {
                var invocationParameter = invocationParametersAsList[i];
                Logger.LogDebug($"Checking invocationParameter '{invocationParameter}'");
                matches.Add(i, -1);

                //What was the last set up parameter matched?
                var startAt = matches.Any() ? matches.Max(x => x.Value) + 1 : 0;
                Logger.LogDebug($"startAt: {startAt}");

                for (var j = 0; j < setUpParametersAsList.Count; j++)
                {
                    var setUpParameter = setUpParametersAsList[j];
                    Logger.LogDebug($"Checking setUpParameter '{setUpParameter}'");

                    if (invocationParameter is DbParameter dbInvocationParameter &&
                        setUpParameter is DbParameter dbSetUpParameter &&
                        DoesInvocationParameterMatchSetUpParameter(dbSetUpParameter, dbInvocationParameter))
                    {
                        matches[i] = j;
                        break;
                    }

                    if (DoesInvocationParameterValueMatchSetUpParameterValue(setUpParameter, invocationParameter))
                    {
                        matches[i] = j;
                        break;
                    }
                }
            }

            Logger.LogDebug($"Match summary '{string.Join(Environment.NewLine, matches.Select(x => $"{x.Key}: {x.Value}"))}'");

            return matches.Count(x => x.Value > -1) >= setUpParametersAsList.Count;
        }

        private static bool DoesInvocationParameterValueMatchSetUpParameterValue(object setUpParameter, object invocationParameter)
        {
            if (invocationParameter == setUpParameter)
            {
                return true;
            }

            if (invocationParameter != null && setUpParameter != null && invocationParameter.Equals(setUpParameter))
            {
                return true;
            }

            if (invocationParameter is string stringInvocationParameterValue &&
                setUpParameter is string stringSetUpParameterValue &&
                stringInvocationParameterValue.Equals(stringSetUpParameterValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static bool DoesInvocationParameterMatchSetUpParameter(IDataParameter setUpParameter, IDataParameter invocationParameter)
        {
            var setUpParameterParameterName = setUpParameter.ParameterName ?? string.Empty;
            var invocationParameterParameterName = invocationParameter.ParameterName ?? string.Empty;

            if (invocationParameterParameterName.Equals(setUpParameterParameterName, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            return DoesInvocationParameterValueMatchSetUpParameterValue(setUpParameter.Value, invocationParameter.Value);
        }

        /// <summary>
        ///     Converts a sequence of invocation parameters to a string of parameter names and values.
        /// </summary>
        /// <param name="invocationParameters">The invocation parameters.</param>
        /// <returns>A string of parameter names and values.</returns>
        public static string StringifyParameters(IEnumerable<object> invocationParameters)
        {
            var invocationParametersAsList = invocationParameters.ToList();
            var parts = new List<string>();
            for (var i = 0; i < invocationParametersAsList.Count; i++)
            {
                var invocationParameter = invocationParametersAsList[i];

                var sb = new StringBuilder();
                switch (invocationParameter)
                {
                    case DbParameter dbInvocationParameter:
                    {
                        sb.Append(dbInvocationParameter.ParameterName);
                        sb.Append(": ");
                        if (dbInvocationParameter.Value == null)
                        {
                            sb.Append("null");
                        }
                        else
                        {
                            sb.Append(dbInvocationParameter.Value);
                        }

                        break;
                    }

                    case null:
                        sb.Append("Parameter ");
                        sb.Append(i);
                        sb.Append(": null");
                        break;

                    default:
                        sb.Append("Parameter ");
                        sb.Append(i);
                        sb.Append(": ");
                        sb.Append(invocationParameter);
                        break;
                }

                parts.Add(sb.ToString());
            }

            return string.Join(Environment.NewLine, parts);
        }
    }
}