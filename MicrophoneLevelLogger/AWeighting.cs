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
                new(10, -70.4),
                new(12.5, -63.4),
                new(16, -56.7),
                new(20, -50.5),
                new(25, -44.7),
                new(31.5, -39.4),
                new(40, -34.6),
                new(50, -30.2),
                new(63, -26.2),
                new(80, -22.5),
                new(100, -19.1),
                new(125, -16.1),
                new(160, -13.4),
                new(200, -10.9),
                new(250, -8.6),
                new(315, -6.6),
                new(400, -4.8),
                new(500, -3.2),
                new(630, -1.9),
                new(800, -0.8),
                new(1000, 0),
                new(1250, 0.6),
                new(1600, 1),
                new(2000, 1.2),
                new(2500, 1.3),
                new(3150, 1.2),
                new(4000, 1),
                new(5000, 0.5),
                new(6300, -0.1),
                new(8000, -1.1),
                new(10000, -2.5),
                new(12500, -4.3),
                new(16000, -6.6),
                new(20000, -9.3)
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
        var maxDecibel = Decibel.Minimum.AsPrimitive();
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
                maxDecibel = Decibel.Minimum.AsPrimitive();
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