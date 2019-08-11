using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EntityFrameworkCore.Testing.Common {
    public class SqlParameterParameterNameAndValueEqualityComparer : EqualityComparer<SqlParameter> {
        public override bool Equals(SqlParameter x, SqlParameter y) {
            var parameterNamesAreEqual = false;
            if (x.ParameterName == null && y.ParameterName == null)
                parameterNamesAreEqual = true;
            else if (x.ParameterName != null || y.ParameterName != null)
                parameterNamesAreEqual = x.ParameterName.Equals(y.ParameterName, StringComparison.CurrentCultureIgnoreCase);

            var valuesAreEqual = false;
            if (x.Value == null && y.Value == null)
                valuesAreEqual = true;
            else if (x.Value != null || y.Value != null)
                valuesAreEqual = x.Value.ToString().Equals(y.Value.ToString(), StringComparison.CurrentCultureIgnoreCase);

            return parameterNamesAreEqual && valuesAreEqual;
        }

        public override int GetHashCode(SqlParameter obj) {
            var hashCode = obj.ParameterName.ToLower().GetHashCode();
            if (obj.Value != null)
                hashCode += obj.Value.ToString().ToLower().GetHashCode();
            return hashCode;
        }
    }
}