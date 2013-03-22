﻿// <copyright file="MvxAccelerometer.cs" company="Cirrious">
// (c) Copyright Cirrious. http://www.cirrious.com
// This source is subject to the Microsoft Public License (Ms-PL)
// Please see license.txt on http://opensource.org/licenses/ms-pl.html
// All other rights reserved.
// </copyright>
//  
// Project Lead - Stuart Lodge, Cirrious. http://www.cirrious.com - Hire me - I'm worth it!

using System;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Exceptions;
using Windows.Devices.Sensors;

namespace Cirrious.MvvmCross.Plugins.Accelerometer.WindowsStore
{
    public class MvxAccelerometer : IMvxAccelerometer
    {
        private Windows.Devices.Sensors.Accelerometer _accelerometer;

        public void Start()
        {
            if (_accelerometer != null)
            {
                throw new MvxException("Accelerometer already started");
            }
            _accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault();
            _accelerometer.ReadingChanged += AccelerometerOnReadingChanged;
        }

        public void Stop()
        {
            if (_accelerometer == null)
            {
                throw new MvxException("Accelerometer not started");
            }
            _accelerometer.ReadingChanged -= AccelerometerOnReadingChanged;
            _accelerometer = null;
        }

        private void AccelerometerOnReadingChanged(Windows.Devices.Sensors.Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            var handler = ReadingAvailable;

            if (handler == null)
                return;

            var reading = ToReading(args.Reading);

            handler(this, new MvxValueEventArgs<MvxAccelerometerReading>(reading));
        }

        private static MvxAccelerometerReading ToReading(AccelerometerReading sensorReading)
        {
            var reading = new MvxAccelerometerReading
                {
                    X = sensorReading.AccelerationX,
                    Y = sensorReading.AccelerationY,
                    Z = sensorReading.AccelerationZ,
                };
            return reading;
        }

        public bool Started
        {
            get { return _accelerometer != null; }
        }

        public MvxAccelerometerReading LastReading
        {
            get
            {
                try
                {
                    var reading = ToReading(_accelerometer.GetCurrentReading());
                    return reading;
                }
                catch (Exception exception)
                {
                    throw exception.MvxWrap("Problem getting current Accelerometer reading");
                }
            }
        }

        public event EventHandler<MvxValueEventArgs<MvxAccelerometerReading>> ReadingAvailable;
    }
}