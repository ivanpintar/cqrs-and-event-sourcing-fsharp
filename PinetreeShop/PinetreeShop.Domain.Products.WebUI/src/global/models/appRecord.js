import { Record, List } from 'immutable';

export default new Record({
    products: List(),
    filter: '',
    toastr: { toastrs: [] }
});