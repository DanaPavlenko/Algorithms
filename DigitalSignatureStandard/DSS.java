/*
 * @file   DSS.java
 * @author Dana
 *
 */

public class Sign {
	public static BigInteger paramQ;
	public static BigInteger paramP;
    public static BigInteger paramT;
    public static BigInteger paramG;

    //private key
    private BigInteger paramX;
    private static BigInteger paramY;

    public Sign(BigInteger q, BigInteger p)
    {
        paramQ = q;
        paramP = p;
        paramT = (p.subtract(BigInteger.ONE)).divide(q);
        SetParamG();
        paramX = GetRandom(paramQ);
        paramY = paramG.modPow(paramX, paramP);
    }

    private BigInteger GetRandom(BigInteger maxValue)
    {
        Random rand = new Random();
        int tmp1 = rand.nextInt();
        int tmp2 = rand.nextInt();
        int tmp3 = rand.nextInt();

        BigInteger b1 = BigInteger.valueOf(tmp1 * Integer.MAX_VALUE);
        BigInteger b2 = BigInteger.valueOf(tmp2 * Integer.MAX_VALUE);
        BigInteger b3 = BigInteger.valueOf(10).modPow(BigInteger.valueOf(tmp3), paramQ);
        BigInteger result = (b1.multiply(b3).add(b2).mod((paramQ.subtract(BigInteger.ONE))));
        if (result == BigInteger.ZERO) {
            return BigInteger.ONE;
        }
        return result;
    }

    private void SetParamG()
    {
        BigInteger h = BigInteger.ONE;
        do {
            h = h.add(BigInteger.ONE);
            paramG = h.modPow(paramT, paramP);
        } while (paramG.intValue() < 2);
    }

    public StringBuilder GenerateSign(String text)
    {
        BigInteger H = GetHashCode(text);
        BigInteger k;
        BigInteger r;
        BigInteger s;
        do {
            do {
                k = GetRandom(paramQ);
                r = paramG.modPow(k, paramP).mod(paramQ);
            } while (r == BigInteger.ZERO);

            BigInteger s1 = k.modPow(paramQ.subtract(BigInteger.valueOf(2)),paramQ);
            BigInteger s2 = H.add(paramX).multiply(r).mod(paramQ);
            s = s1.multiply(s2).mod(paramQ); 
        } while (s == BigInteger.ZERO);

        StringBuilder SB = new StringBuilder();
        SB.append(r.toString());
        SB.append(",");
        SB.append(s.toString());

        return SB;
    }
    public static String[] CheckSign(String text, String in_r, String in_s, String message)
    {
        BigInteger r = BigInteger.valueOf(Long.parseLong(in_r));
        BigInteger s = BigInteger.valueOf(Long.parseLong(in_s));
        BigInteger H = GetHashCode(text);
       
        BigInteger w = s.modPow(paramQ.subtract(BigInteger.valueOf(2)), paramQ);
        BigInteger u1 = H.multiply(w).mod(paramQ);
        BigInteger u2 = r.multiply(w).mod(paramQ);

        BigInteger v1 = paramG.modPow(u1, paramP);
        BigInteger v2 = paramY.modPow(u2, paramP);
        BigInteger v3 = v1.multiply(v2).divide(paramP);

        BigInteger v = v3.mod(paramQ);
        String[] s_ret = {v.toString(), r.toString()}; 
        
        return s_ret;
    }
    
    private static BigInteger GetHashCode(String text)
    {
        SHA1 hash = new SHA1();
        String str = hash.digest(text.getBytes());

    	byte[] hashBYTE = str.getBytes();
        BigInteger hashINT = new BigInteger(hashBYTE);

        BigInteger result = hashINT.and(paramQ);
        return result;
    } 
}
