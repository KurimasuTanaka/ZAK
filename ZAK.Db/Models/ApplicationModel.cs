﻿using System.ComponentModel.DataAnnotations;
using BlazorApp.Enums;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZAK.Db.Models;


    public class ApplicationModel 
    {
        [Key]
        public int id {get; set;}

        public AddressModel? address { get; set; }
        public int year {get; set;} = 0;
        public int month {get; set;} = 0;
        public int day {get; set;} = 0;

        public int maxDaysForConnection {get; set;}

        public StretchingStatus stretchingStatus {get; set;} = StretchingStatus.NotSctreched;

        //Possible time window for tomorrow
        public bool timeRangeIsSet {get;set;} = true;
        public bool secondPart {get;set;} = false;
        public bool firstPart {get;set;} = false;
        public int startHour {get; set;} = 10;
        public int endHour {get; set;} = 19;

        //Tariff change application
        public bool tarChangeApp {get; set;} = false;

        //Did client asked about the status of the application
        public bool statusWasChecked {get; set;} = false;
        //Full comments
        public string operatorComment {get; set;} = String.Empty; 
        public string masterComment {get; set;} = String.Empty;

        //Application is added to the schedule
        public bool inSchedule {get;set;} = false;

        //Application have hight priority
        public bool important {get;set;} = false;

        //Free cable is available
        public bool freeCable { get; set; } = false;

        //Application is urgent
        public bool urgent {get; set;} = false;

        //Application can be ignored
        public bool ignored { get; set; } = false;
        //Application was updated in last update
//        public bool applicationWasUpdated { get; set; } = false;
        public bool addresWasUpdated { get; set; } = false;
        public bool masterCommentWasUpdated { get; set; } = false;
        public bool operatorCommentWasUpdated { get; set; } = false;
        public bool statusWasUpdated { get; set; } = false;


        public ApplicationModel() { }
        public ApplicationModel(ApplicationModel applicationModel)
        {
            if(applicationModel is null)
            {
                return;
            }

            foreach (PropertyInfo property in typeof(ApplicationModel).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(this, property.GetValue(applicationModel));
            }    
        }
    }

