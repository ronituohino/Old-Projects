using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Status")]
public class Status : ScriptableObject
{
    public Sprite sprite;

    public Character.CharacterData.Stats statEffects;
}