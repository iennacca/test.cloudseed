import OptionResult from "./OptionResult"

export interface ITryOptionResult<T> {
    some: boolean,
    none: boolean,
    value: T | null
    error: string | null
}

export default class TryOptionResult<T> implements ITryOptionResult<T> {
    get some(): boolean {
        return this._optionResult.some
    }
    get none(): boolean {
        return this._optionResult.none
    }
    get value(): T | null {
        return this._optionResult.value
    }
    error: string | null
    private _optionResult: OptionResult<T>

    private constructor(option: OptionResult<T>, error: string | null) {
        this._optionResult = option
        this.error = error
    }

    public static createSome<T>(value: T): TryOptionResult<T> {
        var option = OptionResult.createSome<T>(value)
        return new TryOptionResult<T>(
            option,
            null
        )
    }

    public static createNone<T>(error: string): TryOptionResult<T> {
        var option = OptionResult.createNone<T>()
        return new TryOptionResult<T>(
            option,
            error
        )
    } 
}