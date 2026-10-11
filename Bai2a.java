package Bai2;

import java.io.*;

public class Bai2a {

    // Kiểm tra chuỗi có phải số thuận nghịch không
    static boolean isPalindrome(String s) {
        int l = 0, r = s.length() - 1;
        while (l < r) {
            if (s.charAt(l) != s.charAt(r)) return false;
            l++; r--;
        }
        return true;
    }

    // Tổng các chữ số trong chuỗi
    static int digitSum(String s) {
        int sum = 0;
        for (char c : s.toCharArray()) sum += (c - '0');
        return sum;
    }

    // Chuỗi không chứa chữ số 0
    static boolean noZero(String s) {
        return !s.contains("0");
    }

    public static void main(String[] args) throws Exception {
        PrintWriter out = new PrintWriter("Loai1.out");
        int dem = 0;

        // Format: 091N.abc.cba, N: 2..8, a,b,c: 0..9
        for (int N = 2; N <= 8; N++) {
            for (int a = 0; a <= 9; a++) {
                for (int b = 0; b <= 9; b++) {
                    for (int c = 0; c <= 9; c++) {

                        String phone = String.format("091%d.%d%d%d.%d%d%d", N, a, b, c, c, b, a);
                        String tail  = "" + a + b + c + c + b + a; // 6 chữ số đuôi
                        String last5 = "" + b + c + c + b + a;     // 5 chữ số cuối

                        boolean loaiI   = isPalindrome(last5);
                        boolean loaiII  = loaiI  && (digitSum(tail) % 10 == 0);

                        // Bài 2a: Loại I KHÔNG thuộc Loại II
                        if (loaiI && !loaiII) {
                            out.print(phone + " ");
                            dem++;
                            if (dem % 8 == 0) out.println();
                        }
                    }
                }
            }
        }

        out.close();
    }
}
