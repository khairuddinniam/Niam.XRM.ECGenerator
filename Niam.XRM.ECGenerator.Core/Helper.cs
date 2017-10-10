using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xrm.Sdk.Metadata;

namespace Niam.XRM.ECGenerator.Core
{
    public static class Helper
    {
        public static bool IsUserOrTeamOwnership(this EntityMetadata metadata)
            => metadata.OwnershipType == OwnershipTypes.UserOwned ||
               metadata.OwnershipType == OwnershipTypes.TeamOwned;

        public static bool IsOrganizationOwnership(this EntityMetadata metadata)
            => metadata.OwnershipType == OwnershipTypes.OrganizationOwned;

        public static string AdjustName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return null;

            // trim off leading and trailing whitespace
            name = name.Trim();
            if (String.IsNullOrWhiteSpace(name)) return null;

            // should deal with spaces => camel casing;
            name = name.Dehumanize();
            if (String.IsNullOrWhiteSpace(name)) return null;

            var sb = new StringBuilder();
            if (!SyntaxFacts.IsIdentifierStartCharacter(name[0]))
                sb.Append("_"); // the first characters 

            foreach (var ch in name)
            {
                if (SyntaxFacts.IsIdentifierPartCharacter(ch))
                    sb.Append(ch);
            }

            var result = sb.ToString();

            if (SyntaxFacts.GetKeywordKind(result) != SyntaxKind.None)
                result = @"@" + result;

            return result;
        }
        
        public static IEnumerable<AttributeMetadata> FilterAttributes(IEnumerable<AttributeMetadata> attributes, string baseClassName)
        {
            if (baseClassName == EntityBaseClass.Entity) return attributes;

            if (baseClassName == EntityBaseClass.UserOrTeam)
                return attributes.Where(a => UserOrTeamExcludeAttributeSchemaNames.All(ex => ex != a.SchemaName));

            if (baseClassName == EntityBaseClass.Organization)
                return attributes.Where(a => OrganizationExcludeAttributeSchemaNames.All(ex => ex != a.SchemaName));

            return attributes;
        }

        public static readonly string[] OrganizationExcludeAttributeSchemaNames =
        {
            "CreatedOn",
            "ModifiedOn",
            "VersionNumber",
            "CreatedBy",
            "CreatedOnBehalfBy",
            "ImportSequenceNumber",
            "ModifiedBy",
            "ModifiedOnBehalfBy",
            "OverriddenCreatedOn",
            "statecode",
            "statuscode",
            "TimeZoneRuleVersionNumber",
            "UTCConversionTimeZoneCode"
        };

        public static readonly string[] UserOrTeamExcludeAttributeSchemaNames =
        {
            "CreatedOn",
            "ModifiedOn",
            "VersionNumber",
            "CreatedBy",
            "CreatedOnBehalfBy",
            "ImportSequenceNumber",
            "ModifiedBy",
            "ModifiedOnBehalfBy",
            "OverriddenCreatedOn",
            "statecode",
            "statuscode",
            "TimeZoneRuleVersionNumber",
            "UTCConversionTimeZoneCode",

            "OwningBusinessUnit",
            "OwningTeam",
            "OwningUser",
            "OwnerId"
        };
    }
}
