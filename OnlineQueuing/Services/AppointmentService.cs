﻿using OnlineQueuing.Data;
using OnlineQueuing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineQueuing.DTO;

namespace OnlineQueuing.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationContext applicationContext;
        private readonly IAuthService authService;

        public AppointmentService(ApplicationContext applicationContext, IAuthService authService)
        {
            this.applicationContext = applicationContext;
            this.authService = authService;
        }

        public bool CreateAppointment(HttpRequest request, AppointmentDTO appointmentDTO)
        {
            List<User> appUsers = applicationContext.Users.Select(u => u).ToList();
            List<int> allTimeSlots = appUsers.SelectMany(u => u.Appointments.Select(a => a.TimeSlot)).ToList();
            List<string> allDatetime = appUsers.SelectMany(d => d.Appointments.Select(t => t.Date)).ToList();
            Appointment appointment = new Appointment()
            {
                TimeSlot = appointmentDTO.TimeSlot,
                ServiceType = appointmentDTO.ServiceType,
                Date = appointmentDTO.Date
            };
            //List<User> admins = appUsers.Where(u => u.Role.Equals("Admin")).ToList();

            if (!allTimeSlots.Contains(appointment.TimeSlot) && appointment.TimeSlot > 1 && appointment.TimeSlot < 8 && !allDatetime.Contains(appointment.Date))
            {
                string email = authService.GetEmailFromJwtToken(request);
                User user = applicationContext.Users.FirstOrDefault(u => u.Email == email);
                user.Appointments.Add(appointment);
                applicationContext.Add(appointment);
                applicationContext.SaveChanges();
                return true;
            }
            else return false; 
        }
/*
        public bool DeleteAppointment(AppointmentDTO appointmentDTO, HttpRequest request)
        {
            string email = authService.GetEmailFromJwtToken(request);
            User user = applicationContext.Users.FirstOrDefault(u=>u.Email==email);

            List<Appointment> userAppointments = user.Appointments.Select(a => a).ToList();

            if (userAppointments.Contains(appointmentDTO))
            {
                applicationContext.Remove(appointmentDTO);
                applicationContext.SaveChanges();
                return true;
            }
            else return false;
        }
*/
    }
}
