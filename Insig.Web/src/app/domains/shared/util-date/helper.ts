export class Helper {
    static formatDate(date: string | Date, monthly = false): string {
        const d = new Date(date);
        const year = d.getFullYear();
        let month = (d.getMonth() + 1).toString();
        let day = d.getDate().toString();

        if (month.length < 2) {
            month = `0${month}`;
        }
        if (monthly) {
            return [year, month].join('-');
        }
        if (day.length < 2) {
            day = `0${day}`;
        }

        return [year, month, day].join('-');
    }

    static isValidDate(date: string | Date): boolean {
        // eslint-disable-next-line no-restricted-globals
        return !isNaN(new Date(date).getTime());
    }

    static dateWithoutTime(date: string | Date): Date {
        const result = new Date(date);
        result.setHours(0, 0, 0, 0);
        return result;
    }
}
