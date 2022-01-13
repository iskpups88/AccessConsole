using System;
using System.ComponentModel.DataAnnotations;

namespace AccessConsole.Enums
{
    [Flags]
    public enum AccessTypes
    {
        [Display(Name = "Полный запрет")] Deny = 1,

        [Display(Name = "Передача прав")] Grant = 2,

        [Display(Name = "Запись")] Write = 4,

        [Display(Name = "Запись, Передача прав")]
        WriteAndGrant = Write | Grant,

        [Display(Name = "Чтение")] Read = 8,

        [Display(Name = "Чтение, Передача прав")]
        ReadAndGrant = Read | Grant,

        [Display(Name = "Чтение, Запись")] ReadAndWrite = Read | Write,

        [Display(Name = "Полный доступ")] All = Grant | Write | Read
    }
}