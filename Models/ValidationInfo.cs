using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AirandWebAPI.Services.Contract
{

    public class ValidationInfo
    {

        private List<string> invalidNarration = new List<string>();

        public void addInvalidationNarration(string invalidationNarration)
        {
            if (invalidationNarration != null)
            {
                this.invalidNarration.Add(invalidationNarration);
            }
        }

        public void addInvalidationNarration(Exception ex)
        {
            if (ex != null)
            {
                this.invalidNarration.Add(ex.Message);
            }
        }

        public void setInvalidationNarrations(List<string> invalidNarration)
        {
            if (invalidNarration != null)
            {
                this.invalidNarration = invalidNarration;
            }
        }

        public String getConcatInvalidationNarrations()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.invalidNarration.Count; i++)
            {
                sb.Append(invalidNarration[i]);
                sb.Append(",");
            }
            return sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : sb.ToString();
        }

        public Boolean isValid()
        {
            return !this.invalidNarration.Any();
        }

    }
}