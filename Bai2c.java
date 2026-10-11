package Bai2;

import java.io.*;

public class Bai2c {

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
        PrintWriter out1 = new PrintWriter("Loai1.out");
        PrintWriter out2 = new PrintWriter("Loai2.out");
        PrintWriter out3 = new PrintWriter("Loai3.out");

        int dem1 = 0, dem2 = 0, dem3 = 0;

        // Format số điện thoại: 091N.abc.cba
        // N: 2..8, a,b,c: 0..9
        for (int N = 2; N <= 8; N++) {
            for (int a = 0; a <= 9; a++) {
                for (int b = 0; b <= 9; b++) {
                    for (int c = 0; c <= 9; c++) {

                        String phone = String.format("091%d.%d%d%d.%d%d%d", N, a, b, c, c, b, a);

                        // Phần đuôi 6 chữ số (bỏ dấu chấm): a b c c b a
                        String tail = "" + a + b + c + c + b + a;

                        // 5 chữ số CUỐI của toàn bộ số (bỏ dấu chấm): N a b c c b a
                        // -> 5 số cuối: b c c b a
                        String last5 = "" + b + c + c + b + a;

                        // Loại I: 5 số cuối tạo số thuận nghịch
                        boolean loaiI = isPalindrome(last5);

                        // Loại II: Loại I + tổng các chữ số của phần đuôi chia hết cho 10
                        boolean loaiII = loaiI && (digitSum(tail) % 10 == 0);

                        // Loại III: Loại II + phần đuôi không chứa số 0
                        boolean loaiIII = loaiII && noZero(tail);

                        if (loaiI) {
                            out1.print(phone + " ");
                            dem1++;
                            if (dem1 % 8 == 0) out1.println();
                        }
                        if (loaiII) {
                            out2.print(phone + " ");
                            dem2++;
                            if (dem2 % 8 == 0) out2.println();
                        }
                        if (loaiIII) {
                            out3.print(phone + " ");
                            dem3++;
                            if (dem3 % 8 == 0) out3.println();
                        }
                    }
                }
            }
        }

        out1.close();
        out2.close();
        out3.close();
    }
}
