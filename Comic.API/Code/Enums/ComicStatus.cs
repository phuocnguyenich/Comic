using System.ComponentModel;

namespace Comic.Api.Code.Enums
{
    public enum ComicStatus
    {
        [Description("Đang tiến hành")]
        Ongoing,

        [Description("Hoàn thành")]
        Completed,

        [Description("Gián đoạn")]
        Hiatus
    }
}
