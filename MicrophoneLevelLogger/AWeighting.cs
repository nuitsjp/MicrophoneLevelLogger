namespace MicrophoneLevelLogger;

/// <summary>
/// 周波数重み付け特性「A特性」
/// </summary>
public class AWeighting
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static AWeighting Instance { get; } =
        new(
            new Weight[]
            {
                new((Hz) 10, new Decibel(-70.4)),
                new((Hz) 12.5, new Decibel(-63.4)),
                new((Hz) 16, new Decibel(-56.7)),
                new((Hz) 20, new Decibel(-50.5)),
                new((Hz) 25, new Decibel(-44.7)),
                new((Hz) 31.5, new Decibel(-39.4)),
                new((Hz) 40, new Decibel(-34.6)),
                new((Hz) 50, new Decibel(-30.2)),
                new((Hz) 63, new Decibel(-26.2)),
                new((Hz) 80, new Decibel(-22.5)),
                new((Hz) 100, new Decibel(-19.1)),
                new((Hz) 125, new Decibel(-16.1)),
                new((Hz) 160, new Decibel(-13.4)),
                new((Hz) 200, new Decibel(-10.9)),
                new((Hz) 250, new Decibel(-8.6)),
                new((Hz) 315, new Decibel(-6.6)),
                new((Hz) 400, new Decibel(-4.8)),
                new((Hz) 500, new Decibel(-3.2)),
                new((Hz) 630, new Decibel(-1.9)),
                new((Hz) 800, new Decibel(-0.8)),
                new((Hz) 1000, new Decibel(0)),
                new((Hz) 1250, new Decibel(0.6)),
                new((Hz) 1600, new Decibel(1)),
                new((Hz) 2000, new Decibel(1.2)),
                new((Hz) 2500, new Decibel(1.3)),
                new((Hz) 3150, new Decibel(1.2)),
                new((Hz) 4000, new Decibel(1)),
                new((Hz) 5000, new Decibel(0.5)),
                new((Hz) 6300, new Decibel(-0.1)),
                new((Hz) 8000, new Decibel(-1.1)),
                new((Hz) 10000, new Decibel(-2.5)),
                new((Hz) 12500, new Decibel(-4.3)),
                new((Hz) 16000, new Decibel(-6.6)),
                new((Hz) 20000, new Decibel(-9.3))
            }
        );

    /// <summary>
    /// インスタンスを生成する
    /// </summary>
    /// <param name="weights"></param>
    private AWeighting(Weight[] weights)
    {
        Weights = weights;
    }

    /// <summary>
    /// 帯域別の重み付け
    /// </summary>
    public Weight[] Weights { get; }

    /// <summary>
    /// A特性で補正する。
    /// </summary>
    /// <param name="decibelByFrequencies"></param>
    /// <returns></returns>
    public DecibelByFrequency[] Filter(DecibelByFrequency[] decibelByFrequencies)
    {
        var result = new DecibelByFrequency[Weights.Length];

        var weightIndex = 0;
        var currentWeight = Weights[weightIndex];
        var maxDecibel = Decibel.Minimum;
        foreach (var currentByFrequency in decibelByFrequencies)
        {
            if (currentByFrequency.Frequency <= currentWeight.Frequency)
            {
                // 周波数帯域内であった場合、最大値をチェックして必要に応じて更新する
                maxDecibel = maxDecibel < currentByFrequency.Decibel
                    ? currentByFrequency.Decibel
                    : maxDecibel;
            }
            else
            {
                // 周波数帯域が移動した場合、移動前の帯域の最大値を決定する
                result[weightIndex] = new DecibelByFrequency(currentWeight.Frequency, maxDecibel);

                // 次の帯域に移動する
                weightIndex++;
                if (Weights.Length == weightIndex)
                {
                    break;
                }
                currentWeight = Weights[weightIndex];
                maxDecibel = Decibel.Minimum;
            }
        }

        // ループを抜ける最後に最後の帯域を
        if (weightIndex < Weights.Length)
        {
            result[weightIndex] = new DecibelByFrequency(currentWeight.Frequency, maxDecibel);
        }
        return result;
    }

}
