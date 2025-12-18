using System.Collections.Generic;
using UnityEngine;

public class TechCategoryDict : MonoBehaviour
{
  public static readonly Dictionary<TechIDs, Categories> Dict = new Dictionary<TechIDs, Categories>
      {
        { TechIDs.j1, Categories.Attack },
        { TechIDs.j2, Categories.Attack },
        { TechIDs.st, Categories.Attack },
        { TechIDs.ut, Categories.Attack },
        { TechIDs.dt, Categories.Attack },
        { TechIDs.DA, Categories.Attack },
        { TechIDs.ss, Categories.Attack },
        { TechIDs.us, Categories.Attack },
        { TechIDs.ds, Categories.Attack },
        { TechIDs.na, Categories.Attack },
        { TechIDs.fa, Categories.Attack },
        { TechIDs.ba, Categories.Attack },
        { TechIDs.ua, Categories.Attack },
        { TechIDs.da, Categories.Attack },
        { TechIDs.nb_c, Categories.Attack },
        { TechIDs.nb_a, Categories.Attack },
        { TechIDs.sb, Categories.Attack },
        { TechIDs.ub_g, Categories.Attack },
        { TechIDs.ub_a, Categories.Attack },
        { TechIDs.db_g, Categories.Attack },
        { TechIDs.db_a, Categories.Attack },
        { TechIDs.g, Categories.Attack },
        { TechIDs.ga, Categories.Attack },
        { TechIDs.fth, Categories.Attack },
        { TechIDs.bth, Categories.Attack },
        { TechIDs.uth, Categories.Attack },
        { TechIDs.dth, Categories.Attack },
        { TechIDs.fc, Categories.Attack },
        { TechIDs.bc, Categories.Attack },
        { TechIDs.uc, Categories.Attack },
        { TechIDs.dc, Categories.Attack },
        { TechIDs.s, Categories.Shield },
        { TechIDs.nd, Categories.Dodge },
        { TechIDs.sd, Categories.Dodge },
        { TechIDs.ad, Categories.Dodge }
      };
}