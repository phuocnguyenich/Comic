using System.ComponentModel;

namespace Comic.Api.Code.Enums
{
    public enum ComicStatus
    {
        [Description("Tất cả")]
        All = -1,

        [Description("Hoàn thành")]
        Completed,

        [Description("Đang tiến hành")]
        Ongoing,

        [Description("Gián đoạn")]
        Hiatus
    }
}
