﻿/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System.Collections.Generic;
using System.Linq;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class QuestBase : DatabaseObject
    {
        //General
        public new const string DATABASE_TABLE = "quests";
        public new const GameObject OBJECT_TYPE = GameObject.Quest;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        
        public string Name = "New Quest";
        public string StartDesc = "";
        public string EndDesc = "";

        //Requirements
        public int ClassReq = 0;
        public int ItemReq = 0;
        public int LevelReq = 0;
        public int QuestReq = 0;
        public int SwitchReq = 0;
        public int VariableReq = 0;
        public int VariableValue = 0;

        //Tasks
        public List<QuestTask> Tasks = new List<QuestTask>();

        public QuestBase(int id) : base(id)
        {
            
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            StartDesc = myBuffer.ReadString();
            EndDesc = myBuffer.ReadString();
            ClassReq = myBuffer.ReadInteger();
            ItemReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();
            QuestReq = myBuffer.ReadInteger();
            SwitchReq = myBuffer.ReadInteger();
            VariableReq = myBuffer.ReadInteger();
            VariableValue = myBuffer.ReadInteger();

            var MaxTasks = myBuffer.ReadInteger();
            Tasks.Clear();
            for (int i = 0; i < MaxTasks; i++)
            {
                QuestTask Q = new QuestTask();
                Q.Objective = myBuffer.ReadInteger();
                Q.Desc = myBuffer.ReadString();
                Q.Data1 = myBuffer.ReadInteger();
                Q.Data2 = myBuffer.ReadInteger();
                Q.Experience = myBuffer.ReadInteger();
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    Q.Rewards[n].ItemNum = myBuffer.ReadInteger();
                    Q.Rewards[n].Amount = myBuffer.ReadInteger();
                }
                Tasks.Add(Q);
            }

            myBuffer.Dispose();
        }

        public byte[] QuestData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(StartDesc);
            myBuffer.WriteString(EndDesc);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(ItemReq);
            myBuffer.WriteInteger(LevelReq);
            myBuffer.WriteInteger(QuestReq);
            myBuffer.WriteInteger(SwitchReq);
            myBuffer.WriteInteger(VariableReq);
            myBuffer.WriteInteger(VariableValue);

            myBuffer.WriteInteger(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                myBuffer.WriteInteger(Tasks[i].Objective);
                myBuffer.WriteString(Tasks[i].Desc);
                myBuffer.WriteInteger(Tasks[i].Data1);
                myBuffer.WriteInteger(Tasks[i].Data2);
                myBuffer.WriteInteger(Tasks[i].Experience);
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].ItemNum);
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].Amount);
                }
            }

            return myBuffer.ToArray();
        }

        public static QuestBase GetQuest(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (QuestBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((QuestBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return QuestData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }
        public override void Delete()
        {
            Objects.Remove(GetId());
        }
        public static void ClearObjects()
        {
            Objects.Clear();
        }
        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, QuestBase> GetObjects()
        {
            Dictionary<int, QuestBase> objects = Objects.ToDictionary(k => k.Key, v => (QuestBase)v.Value);
            return objects;
        }

        public class QuestTask
        {
            public int Objective = 0;
            public string Desc = "";
            public int Data1 = 0;
            public int Data2 = 0;
            public int Experience = 0;
            public List<QuestReward> Rewards = new List<QuestReward>();

            public QuestTask()
            {
                for (int i = 0; i < Options.MaxNpcDrops; i++)
                {
                    Rewards.Add(new QuestReward());
                }
            }
        }

        public class QuestReward
        {
            public int ItemNum = 0;
            public int Amount = 0;
        }
    }
}
