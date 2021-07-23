using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Constants
{
	public class UpTimer
	{
		int _startTime = Environment.TickCount;
		int _stopTime = 0;

		public UpTimer(bool start = true)
		{
			if (!start) Pause();
		}

		/// <summary>
		/// The getter returns the number of milliseconds since the timer was 
		/// started; the resolution of this property depends on the system timer.
		/// The setter changes the value of the timer.
		/// </summary>
		public int Millisec
		{
			get
			{
				return (_stopTime != 0 ? _stopTime : Environment.TickCount) - _startTime;
			}
			set
			{
				_startTime = (_stopTime != 0 ? _stopTime : Environment.TickCount) - value;
			}
		}

		/// <summary>Restarts the timer from zero (unpausing it if it is paused), 
		/// and returns the number of elapsed milliseconds prior to the reset.</summary>
		public int Restart()
		{
			int millisec = Millisec;
			if (Paused)
				_startTime = Environment.TickCount;
			else
				_startTime += millisec;
			_stopTime = 0;
			return millisec;
		}

		public bool Paused { get { return _stopTime != 0; } }

		public bool Pause()
		{
			if (_stopTime != 0)
				return false; // already paused
			_stopTime = Environment.TickCount;
			if (_stopTime == 0) // virtually impossible, but check anyway
				++_stopTime;
			return true;
		}

		public bool Resume()
		{
			if (_stopTime == 0)
				return false; // already running
			_startTime = Environment.TickCount - (_stopTime - _startTime);
			_stopTime = 0;
			return true;
		}

		/// <summary>Restarts the timer from zero if the specified number of 
		/// milliseconds have passed, and returns the former value of Millisec.</summary>
		/// <returns>If the timer was restarted, this method returns the number of 
		/// elapsed milliseconds prior to the reset. Returns 0 if the timer was not 
		/// reset.</returns>
		/// <remarks>If this method resets a paused timer, it remains paused but 
		/// Millisec is set to zero.</remarks>
		public int ClearAfter(int minimumMillisec)
		{
			int millisec = Millisec;
			if ((uint)millisec < (uint)minimumMillisec)
				return 0;
			else
			{
				if (Paused)
					_stopTime = _startTime |= 1; // Set Millisec to zero
				else
					_startTime += millisec;
				return millisec;
			}
		}
	}
}
