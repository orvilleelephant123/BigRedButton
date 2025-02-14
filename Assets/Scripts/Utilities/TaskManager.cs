/*
 * TaskManager.cs
 */

using UnityEngine;
using System.Collections;

namespace UDB
{
	/// <summary>
	/// This is a new coroutine interface for Unity.
	///
	/// The motivation for this is twofold:
	///
	/// 1. The existing coroutine API provides no means of stopping specific
	///    coroutines; StopCoroutine only takes a string argument, and it stops
	///    all coroutines started with that same string; there is no way to stop
	///    coroutines which were started directly from an enumerator.  This is
	///    not robust enough and is also probably pretty inefficient.
	///
	/// 2. StartCoroutine and friends are MonoBehaviour methods.  This means
	///    that in order to start a coroutine, a user typically must have some
	///    component reference handy.  There are legitimate cases where such a
	///    constraint is inconvenient.  This implementation hides that
	///    constraint from the user.
	///
	/// Example usage:
	///
	/// ----------------------------------------------------------------------------
	/// IEnumerator MyAwesomeTask()
	/// {
	///     while(true) {
	///         Debug.Log("Logcat iz in ur consolez, spammin u wif messagez.");
	///         yield return null;
	////    }
	/// }
	///
	/// IEnumerator TaskKiller(float delay, Task t)
	/// {
	///     yield return new WaitForSeconds(delay);
	///     t.Stop();
	/// }
	///
	/// void SomeCodeThatCouldBeAnywhereInTheUniverse()
	/// {
	///     Task spam = new Task(MyAwesomeTask());
	///     new Task(TaskKiller(5, spam));
	/// }
	/// ----------------------------------------------------------------------------
	///
	/// When SomeCodeThatCouldBeAnywhereInTheUniverse is called, the debug console
	/// will be spammed with annoying messages for 5 seconds.
	///
	/// Simple, really.  There is no need to initialize or even refer to TaskManager.
	/// When the first Task is created in an application, a "TaskManager" GameObject
	/// will automatically be added to the scene root with the TaskManager component
	/// attached.  This component will be responsible for dispatching all coroutines
	/// behind the scenes.
	///
	/// Task also provides an event that is triggered when the coroutine exits.
	/// </summary>

	/// <summary>
	/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
	/// It is an error to attempt to start a task that has been stopped or which has
	/// naturally terminated.
	/// </summary>
	public class Task
	{
		/// <summary>
		/// Returns true if and only if the coroutine is running.  Paused tasks
		/// are considered to be running.
		/// </summary>
		public bool IsRunning {
			get {
				return task.IsRunning;
			}
		}

		/// <summary>
		/// Returns true if and only if the coroutine is currently paused.
		/// </summary>
		public bool IsPaused {
			get {
				return task.IsPaused;
			}
		}

		/// <summary>
		/// Delegate for termination subscribers.  manual is true if and only if
		/// the coroutine was stopped with an explicit call to Stop().
		/// </summary>
		public delegate void FinishedHandler (bool manual);

		/// <summary>
		/// Termination event.  Triggered when the coroutine completes execution.
		/// </summary>
		public event FinishedHandler Finished;

		/// <summary>
		/// Creates a new Task object for the given coroutine.
		///
		/// If autoStart is true (default) the task is automatically started
		/// upon construction.
		/// </summary>
		public Task (IEnumerator c, bool autoStart = true)
		{
			task = TaskManager.CreateTask (c);
			task.Finished += TaskFinished;
			if (autoStart) {
				Start ();
			}
		}

		/// <summary>
		/// Begins execution of the coroutine
		/// </summary>
		public void Start ()
		{
			task.Start ();
		}

		/// <summary>
		/// Discontinues execution of the coroutine at its next yield.
		/// </summary>
		public void Stop ()
		{
			task.Stop ();
		}

		public void Pause ()
		{
			task.Pause ();
		}

		public void Unpause ()
		{
			task.Unpause ();
		}

		void TaskFinished (bool manual)
		{
			FinishedHandler handler = Finished;
			if (handler != null) {
				handler (manual);
			}
		}

		TaskManager.TaskState task;
	}

	class TaskManager : MonoBehaviour
	{
		static TaskManager instance;

		public static TaskState CreateTask (IEnumerator coroutine)
		{
			if (instance == null) {
				GameObject go = new GameObject ("TaskManager");
				instance = go.AddComponent<TaskManager> ();
			}
			return new TaskState (coroutine);
		}

		public class TaskState
		{
			public bool IsRunning {
				get {
					return isRunning;
				}
			}

			public bool IsPaused {
				get {
					return isPaused;
				}
			}

			public delegate void FinishedHandler (bool manual);

			public event FinishedHandler Finished;

			IEnumerator taskCoroutine;
			bool isRunning;
			bool isPaused;
			bool isStopped;

			public TaskState (IEnumerator c)
			{
				taskCoroutine = c;
			}

			public void Pause ()
			{
				isPaused = true;
			}

			public void Unpause ()
			{
				isPaused = false;
			}

			public void Start ()
			{
				isRunning = true;
				instance.StartCoroutine (CallWrapper ());
			}

			public void Stop ()
			{
				isStopped = true;
				isRunning = false;
			}

			IEnumerator CallWrapper ()
			{
				yield return null;
				IEnumerator e = taskCoroutine;
				while (isRunning) {
					if (isPaused) {
						yield return null;
					} else {
						if (e != null && e.MoveNext ()) {
							yield return e.Current;
						} else {
							isRunning = false;
						}
					}
				}
				
				FinishedHandler handler = Finished;
				if (handler != null) {
					handler (isStopped);
				}
			}
		}
	}
}