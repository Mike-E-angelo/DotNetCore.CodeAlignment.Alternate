using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DotNetCore.CodeAlignment.Alternate
{
	public class Program
	{
		static void Main()
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
			                 .Run();
		}
	}

	sealed class Configuration : ManualConfig
	{
		public Configuration()
		{
			Add(Job.MediumRun.With(InProcessToolchain.Instance));
			Add(MemoryDiagnoser.Default);
		}
	}

	[Config(typeof(Configuration))]
	public class TwoBenchmarks
	{
		[Benchmark(Baseline = true)]
		public int[] First() => NativeArray.Default.Get();

		[Benchmark]
		public int[] Second() => ArrayFromLoop.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class First
	{
		[Benchmark]
		public int[] Run() => NativeArray.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class Second
	{
		[Benchmark]
		public int[] Run() => SecondNativeArray.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class Third
	{
		[Benchmark]
		public int[] Run() => ThirdNativeArray.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class Fourth
	{
		[Benchmark]
		public int[] Run() => FourthNativeArray.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class Fifth
	{
		[Benchmark]
		public int[] Run() => ArrayFromLoop.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class ThreeBenchmarks
	{
		[Benchmark]
		public int[] First() => NativeArray.Default.Get();

		[Benchmark]
		public int[] Second() => SecondNativeArray.Default.Get();

		[Benchmark]
		public int[] Third() => ArrayFromLoop.Default.Get();
	}

	[Config(typeof(Configuration))]
	public class FourBenchmarks
	{
		[Benchmark]
		public int[] First() => NativeArray.Default.Get();

		[Benchmark]
		public int[] Second() => SecondNativeArray.Default.Get();

		[Benchmark]
		public int[] Third() => ThirdNativeArray.Default.Get();

		[Benchmark]
		public int[] Fourth() => FourthNativeArray.Default.Get();
	}



	public interface ISource<out T>
	{
		T Get();
	}

	sealed class NativeArray : ISource<int[]>
	{
		public static NativeArray Default { get; } = new NativeArray();

		NativeArray() : this(Select.Default, Data.Default) {}

		readonly Func<string, int> _select;
		readonly string[]          _data;

		public NativeArray(Func<string, int> select, string[] data)
		{
			_select = select;
			_data   = data;
		}

		public int[] Get() => _data.Select(_select).ToArray();
	}

	sealed class SecondNativeArray : ISource<int[]>
	{
		public static SecondNativeArray Default { get; } = new SecondNativeArray();

		SecondNativeArray() : this(Select.Default, Data.Default) {}

		readonly Func<string, int> _select;
		readonly string[]          _data;

		public SecondNativeArray(Func<string, int> select, string[] data)
		{
			_select = select;
			_data   = data;
		}

		public int[] Get() => _data.Select(_select).ToArray();
	}


	sealed class ThirdNativeArray : ISource<int[]>
	{
		public static ThirdNativeArray Default { get; } = new ThirdNativeArray();

		ThirdNativeArray() : this(Select.Default, Data.Default) {}

		readonly Func<string, int> _select;
		readonly string[]          _data;

		public ThirdNativeArray(Func<string, int> select, string[] data)
		{
			_select = select;
			_data   = data;
		}

		public int[] Get() => _data.Select(_select).ToArray();
	}

	sealed class FourthNativeArray : ISource<int[]>
	{
		public static FourthNativeArray Default { get; } = new FourthNativeArray();

		FourthNativeArray() : this(Select.Default, Data.Default) {}

		readonly Func<string, int> _select;
		readonly string[]          _data;

		public FourthNativeArray(Func<string, int> select, string[] data)
		{
			_select = select;
			_data   = data;
		}

		public int[] Get() => _data.Select(_select).ToArray();
	}

	sealed class ArrayFromLoop : ISource<int[]>
	{
		public static ArrayFromLoop Default { get; } = new ArrayFromLoop();

		ArrayFromLoop() : this(Select.Default, Data.Default) {}

		readonly Func<string, int> _select;
		readonly string[]          _data;

		public ArrayFromLoop(Func<string, int> select, string[] data)
		{
			_select = select;
			_data   = data;
		}

		public int[] Get()
		{
			var i = 0;
			var r = new int[_data.Length];
			var s = _select;
			foreach (var d in _data)
				r[i++] = s(d);
			return r;
		}
	}

	public class Source<T> : ISource<T>
	{
		public static implicit operator T(Source<T> source) => source.Get();

		readonly T _instance;

		public Source(T instance) => _instance = instance;

		public T Get() => _instance;
	}

	sealed class Data : Source<string[]>
	{
		public static Data Default { get; } = new Data();

		Data() : base(new Fixture().CreateMany<string>(10_000).ToArray()) {}
	}

	sealed class Select : Source<Func<string, int>>
	{
		public static Select Default { get; } = new Select();

		Select() : this(x => default) {}

		public Select(Expression<Func<string, int>> instance) : base(instance.Compile()) {}
	}

}
