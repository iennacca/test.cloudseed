
export interface IOptionResult<T> {
    some: boolean,
    none: boolean,
    value: T | null
}

export default class OptionResult<T> implements IOptionResult<T> {
    some: boolean
    none: boolean
    value: T | null

    private constructor(value: T | null) {
        if(value !== null) {
            this.value = value;
            this.some = true;
            this.none = false;
        } else {
            this.value = null
            this.some = false 
            this.none = true
        }
    }

    public static createSome<T>(value: T): OptionResult<T> {
        return new OptionResult<T>(
            value
        )
    }

    public static createNone<T>(): OptionResult<T> {
        return new OptionResult<T>(null)
    } 
}