using BukBot.Enums;
using System;

namespace BukBot.Helpers
{
    public static class RoleFactory
    {
        public static string GetRoleName(RoleTypeEnum roleTypeEnum) =>
            roleTypeEnum switch
            {
                RoleTypeEnum.Buk => "Buk",
                RoleTypeEnum.ZastepcaBoka => "Zastepca Boka",
                RoleTypeEnum.Pociong => "Pociong",
                RoleTypeEnum.KrólowaZiemniaków => "Królowa ziemniaków",
                RoleTypeEnum.Dupa => "Dupa",
                _ => throw new Exception("Nie ma takiej roli")
            };
    }
}
