using System.Collections.Generic;

public class TechIdNameDict
{
    public static readonly Dictionary<TechIDs, string> Dict = new Dictionary<TechIDs, string>
    {
        { TechIDs.j1, "弱1段" },
        { TechIDs.j2, "弱2段" },
        { TechIDs.st, "横強" },
        { TechIDs.ut, "上強" },
        { TechIDs.dt, "下強" },
        { TechIDs.DA, "ダッシュアタック" },
        { TechIDs.ss, "横スマ" },
        { TechIDs.us, "上スマ" },
        { TechIDs.ds, "下スマ" },
        { TechIDs.na, "空N" },
        { TechIDs.fa, "空前" },
        { TechIDs.ba, "空後" },
        { TechIDs.ua, "空上" },
        { TechIDs.da, "空下" },
        { TechIDs.nb_c, "NB（タメ）" },
        { TechIDs.nb_a, "NB（攻撃）" },
        { TechIDs.sb, "横B" },
        { TechIDs.ub_g, "上B（地上）" },
        { TechIDs.ub_a, "上B（空中）" },
        { TechIDs.db_g, "下B（地上）" },
        { TechIDs.db_a, "下B（空中）" },
        { TechIDs.g, "つかみ" },
        { TechIDs.ga, "つかみ攻撃" },
        { TechIDs.fth, "前投げ" },
        { TechIDs.bth, "後投げ" },
        { TechIDs.uth, "上投げ" },
        { TechIDs.dth, "下投げ" },
        { TechIDs.fc, "前投げ（前）" },
        { TechIDs.bc, "前投げ（後）" },
        { TechIDs.uc, "前投げ（上）" },
        { TechIDs.dc, "前投げ（下）" },
        { TechIDs.s, "シールド" },
        { TechIDs.nd, "その場回避" },
        { TechIDs.sd, "横回避" },
        { TechIDs.ad, "空中回避" }
    };
}