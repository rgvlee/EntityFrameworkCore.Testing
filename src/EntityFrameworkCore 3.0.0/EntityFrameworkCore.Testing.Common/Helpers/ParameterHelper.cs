using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>A helper for parameter matching.</summary>
    public class ParameterMatchingHelper
    {
        /// <summary>Determines whether the invocation parameters match the set up parameters.</summary>
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

            var matches = new Dictionary<object, object>();
            foreach (var setUpParameter in setUpParametersAsList)
            {
                var startAt = matches.Any() ? invocationParametersAsList.IndexOf(matches.Last().Value) : 0;

                foreach (var invocationParameter in invocationParametersAsList.Skip(startAt))
                {
                    if (invocationParameter is DbParameter dbInvocationParameter &&
                        setUpParameter is DbParameter dbSetUpParameter)
                    {
                        if (dbInvocationParameter.ParameterName == null && dbSetUpParameter.ParameterName != null ||
                            dbInvocationParameter.ParameterName != null && dbSetUpParameter.ParameterName == null ||
                            !dbInvocationParameter.ParameterName.Equals(dbSetUpParameter.ParameterName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            continue;
                        }

                        if (dbInvocationParameter.Value is string stringDbInvocationParameterValue &&
                            dbSetUpParameter.Value is string stringDbSetUpParameterValue)
                        {
                            if (!stringDbInvocationParameterValue.Equals(stringDbSetUpParameterValue, StringComparison.CurrentCultureIgnoreCase))
                            {
                                continue;
                            }
                        }
                        else if (!dbInvocationParameter.Value.Equals(dbSetUpParameter.Value))
                        {
                            continue;
                        }

                        matches.Add(setUpParameter, invocationParameter);
                    }
                    else if (invocationParameter is string stringInvocationParameterValue &&
                             setUpParameter is string stringSetUpParameterValue)
                    {
                        if (stringInvocationParameterValue.Equals(stringSetUpParameterValue, StringComparison.CurrentCultureIgnoreCase))
                        {
                            matches.Add(setUpParameter, invocationParameter);
                        }
                    }
                    else if (invocationParameter.Equals(setUpParameter))
                    {
                        matches.Add(setUpParameter, invocationParameter);
                    }
                }
            }

            return matches.Count == setUpParametersAsList.Count;
        }

        /// <summary>Converts a sequence of invocation parameters to a string of parameter names and values.</summary>
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