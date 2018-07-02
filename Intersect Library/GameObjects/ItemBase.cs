﻿using System;
using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects.Events;

namespace Intersect.GameObjects
{
    public class ItemBase : DatabaseObject<ItemBase>
    {
        [Column("Animation")]
        public Guid AnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation
        {
            get => AnimationBase.Get(AnimationId);
            set => AnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("AttackAnimation")]
        [JsonProperty]
        public Guid AttackAnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation
        {
            get => AnimationBase.Get(AttackAnimationId);
            set => AttackAnimationId = value?.Id ?? Guid.Empty;
        }

        public bool Bound { get; set; }
        public int CritChance { get; set; }
        public int Damage { get; set; }
        public int DamageType { get; set; }

        public ConsumableData Consumable { get; set; }

        public int EquipmentSlot { get; set; }
        public bool TwoHanded { get; set; }
        public EffectData Effect { get; set; }

        
        public int SlotCount { get; set; }

        [Column("Spell")]
        [JsonProperty]
        public Guid SpellId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public SpellBase Spell
        {
            get => SpellBase.Get(SpellId);
            set => SpellId = value?.Id ?? Guid.Empty;
        }

        [Column("Event")]
        [JsonProperty]
        public Guid EventId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public EventBase Event
        {
            get => EventBase.Get(EventId);
            set => EventId = value?.Id ?? Guid.Empty;
        }

        public string Desc { get; set; } = "";
        public string FemalePaperdoll { get; set; } = "";
        public ItemTypes ItemType { get; set; }
        public string MalePaperdoll { get; set; } = "";
        public string Pic { get; set; } = "";
        public int Price { get; set; }

        [Column("Projectile")]
        [JsonProperty]
        public Guid ProjectileId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public ProjectileBase Projectile
        {
            get => ProjectileBase.Get(ProjectileId);
            set => ProjectileId = value?.Id ?? Guid.Empty;
        }

        public int Scaling { get; set; }
        public int ScalingStat { get; set; }
        public int Speed { get; set; }
        public bool Stackable { get; set; }
        public int StatGrowth { get; set; }
        public int Tool { get; set; } = -1;

        [Column("StatsGiven")]
        [JsonIgnore]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(StatsGiven, (int)Stats.StatCount);
            set => StatsGiven = DatabaseUtils.LoadIntArray(value, (int)Stats.StatCount);
        }
        [NotMapped]
        public int[] StatsGiven { get; set; }


        [Column("UsageRequirements")]
        [JsonIgnore]
        public string JsonUsageRequirements
        {
            get => JsonConvert.SerializeObject(UsageRequirements);
            set => UsageRequirements = JsonConvert.DeserializeObject<ConditionLists>(value);
        }

        [NotMapped]
        public ConditionLists UsageRequirements = new ConditionLists();

        public ItemBase() => Initialize();

        [JsonConstructor]
        public ItemBase(Guid id) : base(id) => Initialize();

        private void Initialize()
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[(int)Stats.StatCount];
            Consumable = new ConsumableData();
            Effect = new EffectData();
        }

        public bool IsStackable()
        {
            return (ItemType == ItemTypes.Currency || Stackable) && ItemType != ItemTypes.Equipment && ItemType != ItemTypes.Bag;
        }
    }

    public class ConsumableData
    {
        public ConsumableType Type { get; set; }
        public int Value { get; set; }
    }

    public class EffectData
    {
        public EffectType Type { get; set; }
        public int Percentage { get; set; }
    }
}