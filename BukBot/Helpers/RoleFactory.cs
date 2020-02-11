using BukBot.Enums;
using System;

namespace BukBot.Helpers
{
    public static class RoleFactory
    {
        public static string GetRoleName(RoleTypeEnum roleTypeEnum)
        {

            switch (roleTypeEnum)
            {
                case RoleTypeEnum.Buk:
                    return "Buk";
                case RoleTypeEnum.ZastepcaBoka:
                    return "Zastepca Boka";
                case RoleTypeEnum.Pociong:
                    return "Pociong";
                case RoleTypeEnum.KrólowaZiemniaków:
                    return "Królowa ziemniaków";
                case RoleTypeEnum.Liga:
                    return "Liga";
                default:
                    throw new Exception("Nie ma takiej roli");
                };
        }
        //roleTypeEnum switch
        //    {
        //        RoleTypeEnum.Buk => "Buk",
        //        RoleTypeEnum.ZastepcaBoka => "Zastepca Boka",
        //        RoleTypeEnum.Pociong => "Pociong",
        //        RoleTypeEnum.KrólowaZiemniaków => "Królowa ziemniaków",
        //        RoleTypeEnum.Liga => "Liga",
        //        _ => throw new Exception("Nie ma takiej roli")
        //    };
    }

}
